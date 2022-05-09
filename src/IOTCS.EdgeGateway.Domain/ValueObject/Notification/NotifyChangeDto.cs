using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject.Notification
{
    public class NotifyChangeDto
    {
        public string DeviceID { get; set; } = string.Empty;

        /// <summary>
        /// 键值对
        /// </summary>
        public List<NotifyChangeVariableDto> Nodes { get; set; }
    }
}
