using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.ComResDriver
{
    public interface IResourceDriver
    {
        dynamic Parameter { get; set; }

        void Initialize(string config);

        bool IsConnected();

        Task<bool> Run(dynamic data);
    }
}
