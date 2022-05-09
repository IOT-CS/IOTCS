using IOTCS.EdgeGateway.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IDeviceConfigRepository
    {

        Task<DeviceConfigModel> GetAllDeviceConfigByDeviceId(string deviceId);

        Task<bool> Create(DeviceConfigModel configDto);

        Task<bool> Update(DeviceConfigModel configDto);

        Task<IEnumerable<DeviceConfigModel>> GetAsync();
    }
}
