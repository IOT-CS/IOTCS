using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject.Device
{
    public class DeviceConfigDto
    {
        public string Id { get; set; }
        public string ConfigJson { get; set; }
        public string DeviceId { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public string DriveType { get; set; } = "OPCUA";

    }
}
