using MediatR;

namespace IOTCS.EdgeGateway.Commands
{
    public class OpcUaCommand : INotification
    {
        /// <summary>
        /// 主体数据
        /// </summary>
        public string Message { get; set; }
    }
}
