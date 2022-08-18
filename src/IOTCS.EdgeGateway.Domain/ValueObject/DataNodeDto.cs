using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class DataNodeDto
    {
        public string Tag { get; set; }

        public string FieldName { get; set; }

        public string NodeId { get; set; }

        public string NodeValue { get; set; }

        public string StatusCode { get; set; }
    }
}
