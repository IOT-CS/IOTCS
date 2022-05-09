using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject.Notification
{
    public class MonitorVariableValueDto
    {
        public string Id { get; set; }

        public string Source { get; set; }

        public string Sink { get; set; }

        public string Status { get; set; }
    }
}
