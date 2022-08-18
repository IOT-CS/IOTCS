using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Diagnostics
{
    public interface IUINotification
    {
        void Publish(string message, string deviceID, string groupID);
    }
}
