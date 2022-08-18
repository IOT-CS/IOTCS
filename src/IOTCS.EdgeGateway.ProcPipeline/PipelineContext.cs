using IOTCS.EdgeGateway.BaseProcPipeline;
using IOTCS.EdgeGateway.ComResDriver;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace IOTCS.EdgeGateway.ProcPipeline
{
    public class PipelineContext : IPipelineContext, IDisposable
    {
        private NetMQActor _actor;
        private PairSocket _shim;
        private NetMQPoller _poller;
        private IMediator _mediator;
        private readonly ILogger _logger;
        private IConcurrentList<RelationshipDto> relationships;
        private ConcurrentDictionary<string, IResourceDriver> resDrivers;
        private readonly ISystemDiagnostics _diagnostics;
        private readonly IUINotification _uINotification;

        public PipelineContext()
        {
            _actor = NetMQActor.Create(RunActor);
            _mediator = IocManager.Instance.GetService<IMediator>();
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _uINotification = IocManager.Instance.GetService<IUINotification>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            relationships = IocManager.Instance.GetService<IConcurrentList<RelationshipDto>>();
            resDrivers = IocManager.Instance.GetService<ConcurrentDictionary<string, IResourceDriver>>();
        }

        private void RunActor(PairSocket shim)
        {
            this._shim = shim;
            this._shim.ReceiveReady += _shim_ReceiveReady;
            this._shim.SignalOK();
            _poller = new NetMQPoller { shim };
            _poller.Run();
        }

        private void _shim_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            string command = e.Socket.ReceiveFrameString();
            _logger.Info($"command => {command}");
            switch (command)
            {
                case NetMQActor.EndShimMessage:
                    _poller.Stop();
                    break;
                default:                    
                    MessageRouter(command);
                    break;
            }
        }

        private void MessageRouter(string message)
        {
            try
            {
                var router = JsonConvert.DeserializeObject<RouterMessage>(message);
                var routerMsg = JsonConvert.DeserializeObject<dynamic>(router.Message);
                var topic = Convert.ToString(routerMsg.Topic);
                var deviceID = Convert.ToString(routerMsg.DeviceID);
                var groupID = Convert.ToString(routerMsg.GroupID);
                _uINotification.Publish(router.OriginMessage, deviceID, groupID);
                _diagnostics.Publish(router.Message);              
                var reslationship = relationships.Where(d => d.Topic.Equals(topic)).ToList();
                if (relationships != null && relationships.Count > 0)
                {
                    foreach (var res in reslationship)
                    {
                        if (resDrivers.ContainsKey(res.ResourceId))
                        {
                            var driver = resDrivers[res.ResourceId];
                            driver.Run(routerMsg);
                        }
                        else
                        {
                            var msg = $"数据处理异常！Message => {routerMsg.Message},未找到北向资源驱动！数据已舍弃";
                            _diagnostics.PublishDiagnosticsInfo(msg);
                        }
                    }
                }
                else
                {
                    var msg = $"数据处理异常！Message => {routerMsg.Message},未找到关联北向资源！数据已舍弃";
                    _diagnostics.PublishDiagnosticsInfo(msg);
                }
            }
            catch (Exception e)
            {
                var msg = $"消息总线路由异常，请检查,异常消息=>{e.Message}, 异常调用栈 => {e.StackTrace}";
                _logger.Info(msg);
                _diagnostics.PublishDiagnosticsInfo(msg);
            }
        }

        public void SendPayload(RouterMessage router)
        {
            if (_actor != null)
            {
                var message = new NetMQMessage();
                var sendMsg = JsonConvert.SerializeObject(router);
                message.Append(sendMsg);
                _actor.SendMultipartMessage(message);
            }
            else
            {
                var msg = "Actor == null，请检查代码。";
                _diagnostics.PublishDiagnosticsInfo(msg);
            }
        }

        public void Dispose()
        {
            if (_actor != null)
            {
                _actor.Dispose();
            }
            if (_shim != null)
            {
                _shim.Close();
                _shim.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
