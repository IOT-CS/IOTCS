using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IDeviceRepository
    {
        Task<bool> Create(DeviceModel deviceModel);

        Task<bool> Update(DeviceModel deviceModel);

        Task<bool> Delete(DeviceModel deviceModel);

        Task<IEnumerable<DeviceModel>> GetDeviceGroup();

        Task<IEnumerable<DeviceDto>> GetAllDevice();

        Task<IEnumerable<DeviceModel>> GetAsync();

    }
}
