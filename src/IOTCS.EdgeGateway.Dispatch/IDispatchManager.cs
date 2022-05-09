using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Dispatch
{
    public interface IDispatchManager
    {
        Task RunTaskAsync(dynamic data);
    }
}
