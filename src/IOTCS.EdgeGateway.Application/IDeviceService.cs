using IOTCS.EdgeGateway.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IDeviceService
    {
        public Task<bool> Create(DeviceDto deviceDto);


        Task<bool> Update(DeviceDto deviceModel);

        Task<bool> Delete(DeviceDto deviceModel);

        Task<IEnumerable<DeviceDto>> GetDeviceGroup();

        Task<IEnumerable<DeviceDto>> GetAllDevice();

        Task<IEnumerable<DeviceDto>> GetAsync();
    }
}
