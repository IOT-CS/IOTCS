using DynamicExpresso;
using IOTCS.EdgeGateway.Commands;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Notification;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.WsHandler;
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
        private readonly WsMessageHandler _webSocket;
        private ConcurrentDictionary<string, NotifyChangeDto> _keyValues;
        private IConcurrentList<DataLocationDto> _dataLocations = null;        

        public UINotificationHandler()
        {
            _logger = IocManager.Instance.GetService<ILogger>();
            _webSocket = IocManager.Instance.GetService<WsMessageHandler>();
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
                    SentToUI(message, notification.DeviceID, notification.GroupID);
                }
            }
            catch (Exception e)
            {
                _logger.Error($"OPCUA 系统消息解析异常，错误消息 => {e.Message},错误位置 => {e.StackTrace},");
            }

            return Task.CompletedTask;
        }

        private void SentToUI(IEnumerable<DataNodeDto> nodes, string deviceID, string groupID)
        {
            String jsonConvertString = string.Empty;            

            if (_keyValues.ContainsKey(groupID))
            {
                double result = 0;
                NotifyChangeDto notify = _keyValues[groupID];
                NotifyChangeVariableDto location = null;
                Interpreter interpreter = new Interpreter();
                foreach (var node in nodes)
                {
                    location = notify.Nodes.Where(w => w.NodeAddress == node.NodeId).FirstOrDefault();
                    if (location != null)
                    {
                        var sinkValue = "0";
                        if (!string.IsNullOrEmpty(location.Expressions) && !string.IsNullOrEmpty(node.NodeValue))
                        {
                            switch (location.NodeType.ToLower())
                            {
                                case "uint8":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(byte), Convert.ToSByte(node.NodeValue)));
                                    break;
                                case "int8":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(sbyte), Convert.ToByte(node.NodeValue)));
                                    break;
                                case "uint16":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(UInt16), Convert.ToUInt16(node.NodeValue)));
                                    break;
                                case "int16":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(Int16), Convert.ToInt16(node.NodeValue)));
                                    break;
                                case "uint32":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(UInt32), Convert.ToUInt32(node.NodeValue)));
                                    break;
                                case "int32":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(Int32), Convert.ToInt32(node.NodeValue)));
                                    break;
                                case "uint64":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(UInt64), Convert.ToUInt64(node.NodeValue)));
                                    break;
                                case "int64":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(Int64), Convert.ToInt64(node.NodeValue)));
                                    break;
                                case "float":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(float), Convert.ToSingle(node.NodeValue)));
                                    break;
                                case "double":
                                    result = interpreter.Eval<double>(location.Expressions.ToString(), new Parameter("raw", typeof(double), Convert.ToDouble(node.NodeValue)));
                                    break;
                            }
                            sinkValue = result.ToString();
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
                              where d.ParentId == groupID
                              select d.ToModel<DataLocationDto, NotifyChangeVariableDto>();
                var notification = new NotifyChangeDto();
                notification.DeviceID = deviceID;
                notification.GroupID = groupID;
                notification.Nodes = new List<NotifyChangeVariableDto>();
                foreach (var n in notifys)
                {
                    n.PropertyChanged += N_PropertyChanged;
                    notification.Nodes.Add(n);
                }
                _keyValues.TryAdd(groupID, notification);
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
                
                Send(monitor).ConfigureAwait(false).GetAwaiter().GetResult();                
            }
            catch (Exception ex)
            {
                _logger.Error($"推送设备变量变化值的内容失败，异常信息 => {ex.Message},位置 => {ex.StackTrace},当前点位=>{JsonConvert.SerializeObject(notify)}");
            }
        }

        private async Task Send(MonitorVariableValueDto message)
        {
            try
            {
                await _webSocket.SendMessageToAllAsync(new WebSocketManager.Common.Message
                {
                    MessageType = WebSocketManager.Common.MessageType.Text,
                    RequestType = "nodeValue",
                    Data = message
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.Error($"推送点位变量信息到界面上，异常信息 => {e.Message},位置 => {e.StackTrace}");
            }
        }
    }
}
