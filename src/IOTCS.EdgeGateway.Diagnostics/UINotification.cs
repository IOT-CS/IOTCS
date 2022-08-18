using IOTCS.EdgeGateway.Logging;
using MediatR;

namespace IOTCS.EdgeGateway.Diagnostics
{
    public class UINotification : IUINotification
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public UINotification(ILogger logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// 发送数据到界面展示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void Publish(string message, string deviceID, string groupID)
        {
            var command = new  Commands.UINotification
            {
                UIMessage = message,
                GroupID = groupID,
                DeviceID = deviceID //DeviceID                
            };
            _mediator.Publish<Commands.UINotification>(command).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
