using IOTCS.EdgeGateway.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.DomainService
{
    public interface IDeviceDomainService
    {
        Task<IEnumerable<DeviceDto>> GetAllDevice ();
    }
}
