using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class RelationshipDto
    {
        public string Id { get; set; }
        public string ResourceId { get; set; }

        public string ResourceName { get; set; }
        public string Topic { get; set; }
        public string CreateTime { get; set; }
        public string CreaterBy { get; set; }
    }
}
