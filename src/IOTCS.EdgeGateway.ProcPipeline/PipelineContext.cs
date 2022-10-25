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
        private NetMQQueue<RouterMessage> _netmqQueue;
        private NetMQPoller _poller;
        private IMediator _mediator;
        private readonly ILogger _logger;
        private IConcurrentList<RelationshipDto> relationships;
        private ConcurrentDictionary<string, IResourceDriver> resDrivers;
        private readonly ISystemDiagnostics _diagnostics;
        private readonly IUINotification _uINotification;

        public PipelineContext()
        {
            initialize();
            _mediator = IocManager.Instance.GetService<IMediator>();
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _uINotification = IocManager.Instance.GetService<IUINotification>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            relationships = IocManager.Instance.GetService<IConcurrentList<RelationshipDto>>();
            resDrivers = IocManager.Instance.GetService<ConcurrentDictionary<string, IResourceDriver>>();
        }

        private void initialize()
        {
            try
            {
                _netmqQueue = new NetMQQueue<RouterMessage>();
                _poller = new NetMQPoller { _netmqQueue };
                _netmqQueue.ReceiveReady += (sender, args) => MessageRouter(_netmqQueue.Dequeue());
                _poller.RunAsync();
            }
            catch (Exception e)
            {
                if (_netmqQueue != null)
                {
                    _netmqQueue.Dispose();
                }
                _netmqQueue = null;
                if (_poller != null)
                {
                    _poller.Dispose();
                }
                _poller = null;

                var msg = $"Pipeline channel 初始化的时候异常，异常信息=>{e.Message}, 异常堆栈 => {e.StackTrace}。";
                _logger.Info(msg);
                _diagnostics.PublishDiagnosticsInfo(msg);
            }
        }

        private void MessageRouter(RouterMessage router)
        {
            try
            {                
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
            if (_netmqQueue != null && _poller != null)
            {
                _netmqQueue.Enqueue(router);
            }
            else
            {
                var msg = "NetMQQueue<RouterMessage> == null 或者 NetMQPoller _poller == null，所以，上层消息总线无法将信息发到指定规则链当中，请检查代码。";
                msg += $"输入信息 =>{JsonConvert.SerializeObject(router)}";
                _logger.Info(msg); 
                _diagnostics.PublishDiagnosticsInfo(msg);
            }
        }

        public void Dispose()
        {
            if (_netmqQueue != null)
            {
                _netmqQueue.Dispose();
            }
            _netmqQueue = null;
            if (_poller != null)
            {
                _poller.Dispose();
            }
            _poller = null;
            GC.SuppressFinalize(this);
        }
    }
}
