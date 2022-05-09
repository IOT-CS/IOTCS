using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class DeviceDto
    {
        public string Id { get; set; }

        public string DeviceName { get; set; }

        public string Description { get; set; }

        public string DriveId { get; set; }

        public string DriveName { get; set; }

        public string DeviceType { get; set; }

        public string CreateTime { get; set; }

        public string CreaterBy { get; set; }

        public string UpdateTime { get; set; }

        public string UpdateBy { get; set; }

        public string ParentId { get; set; }
        public int InUse { get; set; }

        public string Duration { get; set; }
        public string Topic { get; set; }
        public List<DeviceDto> Childrens { get; set; }

       // public bool HasChildren { get; set; }
    }
}
