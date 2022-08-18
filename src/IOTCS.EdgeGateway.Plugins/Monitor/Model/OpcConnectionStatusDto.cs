using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Plugins.Monitor
{
    public class OpcConnectionStatusDto
    {
        public string OpcName { get; set; }

        public string OpcUrl { get; set; }

        
        public DateTime ConnectTime { get; set; }

        public bool IsConnected { get; set; }
    }
}
