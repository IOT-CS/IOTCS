using IOTCS.EdgeGateway.Diagnostics.Notification;
using IOTCS.EdgeGateway.Logging;
using MediatR;

namespace IOTCS.EdgeGateway.Diagnostics
{
    public class SystemDiagnostics : ISystemDiagnostics
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public SystemDiagnostics(ILogger logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// 发送订阅数据到界面展示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void Publish(string message)
        {
            var command = new SystemNotification
            {
                Message = message,
                MsgType = "subLog"
            };
            _mediator.Publish<SystemNotification>(command).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 发送诊断信息到界面展示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void PublishDiagnosticsInfo(string message)
        {
            var command = new SystemNotification 
            {
                Message = message,
                MsgType = "diagnostics"
            };
            _mediator.Publish<SystemNotification>(command).ConfigureAwait(false).GetAwaiter().GetResult();            
        }
    }
}
