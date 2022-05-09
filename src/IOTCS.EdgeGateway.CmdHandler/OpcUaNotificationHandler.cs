using IOTCS.EdgeGateway.Commands;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Dispatch;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.CmdHandler
{
    public class OpcUaNotificationHandler : INotificationHandler<OpcUaNotification>
    {
        private readonly ILogger _logger;
        private readonly IDispatchManager _dispatchManager;

        public OpcUaNotificationHandler()
        {
            _logger = IocManager.Instance.GetService<ILogger>();
            _dispatchManager = IocManager.Instance.GetService<IDispatchManager>();

        }

        public Task Handle(OpcUaNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(notification.Message))
                {
                    var message = JsonConvert.DeserializeObject<SendMessageDto>(notification.Message);
                    ProcessMessage(message.Payload.Values);
                }
                else
                {
                    _logger.Info($"采集的数据为=> 为空");
                }
            }
            catch (Exception e)
            {
                _logger.Error($"数据处理异常 => 位置 => {e.Message},异常 => {e.StackTrace}");
            }

            return Task.CompletedTask;
        }

        private void ProcessMessage(List<dynamic> equipments)
        {
            foreach (var d in equipments)
            {
                _dispatchManager.RunTaskAsync(d);
            }
        }
    }
}
