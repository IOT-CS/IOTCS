using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class DataLocationDto
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string DisplayName { get; set; }

        public string NodePreStamp { get; set; }

        public string NodeAddress { get; set; }

        public int NodeLength { get; set; }

        public string NodeType { get; set; }

        public string Expressions { get; set; }

        public string CreaterBy { get; set; }

        public string CreateTime { get; set; }

        public string UpdateBy { get; set; }

        public string UpdateTime { get; set; }

        public string Source { get; set; }

        public string Sink { get; set; }

        public string Status { get; set; }
    }
}
