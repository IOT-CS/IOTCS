using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Plugins.BACNetDriver
{
    public class BACNetDriver : IBACNetDriver
    {
        public bool Connect(string deviceID)
        {
            throw new NotImplementedException();
        }

        public bool IsAviable()
        {
            throw new NotImplementedException();
        }

        public string Run(string deviceID, string groupID)
        {
            throw new NotImplementedException();
        }
    }
}
