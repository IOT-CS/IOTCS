using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.ComResDriver
{
    public interface IResourceDriver
    {
        dynamic Parameter { get; set; }

        string Initialize(string config);

        string CheckConnected(string config);

        bool IsConnected();

        Task<bool> Run(dynamic data);
    }
}
