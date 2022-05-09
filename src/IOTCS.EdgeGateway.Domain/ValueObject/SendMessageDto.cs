using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class SendMessageDto
    {
        public PayloadMessage Payload { get; set; }
    }

    public class PayloadMessage
    {
        public string ClientID { get; set; } = "OPC UA";

        public List<dynamic> Values { get; set; }
    }
}
