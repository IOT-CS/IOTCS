using DynamicExpresso;
using IOTCS.EdgeGateway.Commands;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Notification;
using IOTCS.EdgeGateway.DotNetty;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.CmdHandler
{
    public class UINotificationHandler : INotificationHandler<UINotification>
    {
        private readonly ILogger _logger;
        private readonly IWebSocketServer _webSocket;
        private ConcurrentDictionary<string, NotifyChangeDto> _keyValues;
        private IConcurrentList<DataLocationDto> _dataLocations = null;

        public UINotificationHandler()
        {
            _logger = IocManager.Instance.GetService<ILogger>();
            _webSocket = IocManager.Instance.GetService<IWebSocketServer>();
            _keyValues = IocManager.Instance.GetService<ConcurrentDictionary<string, NotifyChangeDto>>();
            _dataLocations = IocManager.Instance.GetService<IConcurrentList<DataLocationDto>>();
        }

        public Task Handle(UINotification notification, CancellationToken cancellationToken)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<IEnumerable<DataNodeDto>>(notification.UIMessage);
                if (!string.IsNullOrEmpty(notification.DeviceID))
                {
                    SentToUI(message, notification.DeviceID);
                }                
            }
            catch (Exception e)
            {
                _logger.Error($"OPCUA 系统消息解析异常，错误消息 => {e.Message},错误位置 => {e.StackTrace},");
            }

            return Task.CompletedTask;
        }

        private void SentToUI(IEnumerable<DataNodeDto> nodes, string deviceID)
        {
            String jsonConvertString = string.Empty;            

            if (_keyValues.ContainsKey(deviceID))
            {
                NotifyChangeDto notify = _keyValues[deviceID];
                NotifyChangeVariableDto location = null;
                foreach (var node in nodes)
                {
                    location = notify.Nodes.Where(w => w.NodeAddress == node.NodeId).FirstOrDefault();
                    if (location != null)
                    {
                        var sinkValue = "0";
                        if (!string.IsNullOrEmpty(location.Expressions))
                        {
                            //var result = target.Eval(location.Expressions, new[] { new Parameter("raw", Convert.ToDouble(notify.Source)) });
                            //sinkValue = result.ToString();
                        }
                        else
                        {
                            sinkValue = node.NodeValue;
                        }
                        location.Sink = sinkValue;
                        location.Status = node.StatusCode;
                        location.Source = node.NodeValue;
                    }
                }
            }
            else
            {
                var notifys = from d in _dataLocations
                             where d.ParentId == deviceID
                             select d.ToModel<DataLocationDto, NotifyChangeVariableDto>();
                var notification = new NotifyChangeDto();
                notification.DeviceID = deviceID;
                notification.Nodes = new List<NotifyChangeVariableDto>();
                foreach (var n in notifys)
                {
                    n.PropertyChanged += N_PropertyChanged;
                    notification.Nodes.Add(n);
                }
                _keyValues.TryAdd(deviceID, notification);
            }
        }

        private void N_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var target = new Interpreter();
            var notify = sender as NotifyChangeVariableDto;

            try
            {
                var monitor = new MonitorVariableValueDto
                {
                    Id = notify.Id,
                    Source = notify.Source,
                    Sink = notify.Sink,
                    Status = notify.Status
                };
                var JsonString = HandleMessage(monitor);
                Send(JsonString).ConfigureAwait(false).GetAwaiter().GetResult();
                _logger.Info($"datanodeId => {notify.Id},data=>{JsonString}");
            }
            catch (Exception ex)
            {
                _logger.Error($"推送设备变量变化值的内容失败，异常信息 => {ex.Message},位置 => {ex.StackTrace},当前点位=>{JsonConvert.SerializeObject(notify)}");
            }
        }       

        private async Task Send(string msg)
        {
            try
            {
                var connections = _webSocket?.GetAllConnections();
                if (connections != null && connections.Count > 0)
                {
                    foreach (var conn in connections)
                    {
                        await conn.Send(msg);

                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"推送点位变量信息到界面上，异常信息 => {e.Message},位置 => {e.StackTrace}");
            }
        }

        private string HandleMessage(MonitorVariableValueDto message)
        {
            var result = string.Empty;
            var socketObject = new WebSocketProtocol<MonitorVariableValueDto>();

            if (message != null)
            {
                socketObject.RequestType = "nodeValue";
                socketObject.Data = message;
                result = JsonConvert.SerializeObject(socketObject);
            }

            return result;
        }
    }
}
