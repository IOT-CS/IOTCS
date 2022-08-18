using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.Models
{
    [Table(Name = "tb_drive")]
    public class DriveModel
    {
        public string Id { get; set; }

        public string DriveName { get; set; }
        public string DriveType { get; set; }
        public string DriveParams { get; set; }
        public string Description { get; set; }
        public string CreateTime { get; set; }

        public string CreaterBy { get; set; }
    }
}
