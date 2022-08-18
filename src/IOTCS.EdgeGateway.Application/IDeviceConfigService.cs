using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IDeviceConfigService
    {
        Task<DeviceConfigDto> GetAllDeviceConfigByDeviceId(string deviceId);

        Task<bool> Create(DeviceConfigDto configDto);

        Task<bool> Update(DeviceConfigDto configDto);

        Task<IEnumerable<DeviceConfigDto>> GetAsync();
    }
}
