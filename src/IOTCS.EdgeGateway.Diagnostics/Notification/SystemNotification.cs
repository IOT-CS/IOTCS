using MediatR;

namespace IOTCS.EdgeGateway.Diagnostics.Notification
{
    /// <summary>
    /// 系统通知相关的信息
    /// </summary>
    public class SystemNotification : INotification
    {
        public dynamic Message { get; set; }

        public string MsgType { get; set; } = "";
    }
}
