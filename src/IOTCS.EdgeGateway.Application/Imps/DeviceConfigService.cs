using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Freesql.Helper;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class DeviceConfigService : IDeviceConfigService
    {
        private readonly ILogger _logger;
        private readonly IDeviceConfigRepository _repository;

        public DeviceConfigService(ILogger logger,
                                   IDeviceConfigRepository deviceRepository)
        {
            _logger = logger;
            _repository = deviceRepository;
        }

        public async Task<bool> Create(DeviceConfigDto configDto)
        {
            var result = true;
            var model = configDto.ToModel<DeviceConfigDto, DeviceConfigModel>();

            result = await _repository.Create(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<DeviceConfigDto> GetAllDeviceConfigByDeviceId(string deviceId)
        {
            var result = await _repository.GetAllDeviceConfigByDeviceId(deviceId);

            if (result != null) 
            {
                return result.ToModel<DeviceConfigModel, DeviceConfigDto>();
            }
            return null;
        }

        public async Task<bool> Update(DeviceConfigDto configDto)
        {
            var result = true;
            var model = configDto.ToModel<DeviceConfigDto, DeviceConfigModel>();

            result = await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<DeviceConfigDto>> GetAsync()
        {
            var list = await _repository.GetAsync();

            var query = from s in list
                        select s.ToModel<DeviceConfigModel, DeviceConfigDto>();

            return query;
        }
    }
}
