using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.Models
{
    [Table(Name = "tb_device")]
    public class DeviceModel
    {
        public string Id { get; set; }

        public string DeviceName { get; set; }

        public string Description { get; set; }

        public string DriveId { get; set; }

        public string DeviceType { get; set; }

        public string CreateTime { get; set; }

        public string CreaterBy { get; set; }

        public string UpdateTime { get; set; }

        public string UpdateBy { get; set; }

        public string ParentId { get; set; }

        //是否启用 0关 1开
        public int InUse { get; set; }

        public string Topic { get; set; }

        public string Duration { get; set; }

    }
}
