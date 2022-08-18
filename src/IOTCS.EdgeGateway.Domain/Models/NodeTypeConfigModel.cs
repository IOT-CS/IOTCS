using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.Models
{
    [Table(Name = "tb_nodetypeconfig")]
    public class NodeTypeConfigModel
    {
        public string Id { get; set; }

        public string DriveType { get; set; }

        public string NodeTypeJson { get; set; }
    }
}
