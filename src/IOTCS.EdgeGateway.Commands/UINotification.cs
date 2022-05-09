using MediatR;

namespace IOTCS.EdgeGateway.Commands
{
    public class UINotification :  INotification
    {
        public string DeviceID { get; set; }

        /// <summary>
        /// 主体数据
        /// </summary>
        public string UIMessage { get; set; }
    }
}
