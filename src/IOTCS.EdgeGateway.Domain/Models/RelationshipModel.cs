using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.Models
{
    [Table(Name = "tb_relationship")]
    public class RelationshipModel
    {
        public string Id { get; set; }
        public string ResourceId { get; set; }
        public string Topic { get; set; }
        public string CreateTime { get; set; }
        public string CreaterBy { get; set; }
    }
}
