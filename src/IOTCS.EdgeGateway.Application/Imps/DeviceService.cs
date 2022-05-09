using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using IOTCS.EdgeGateway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class DeviceService : IDeviceService
    {

        private readonly ILogger _logger;
        private readonly IDeviceRepository _repository;


        public DeviceService(IServiceProvider services
            , ILogger logger
            , IDeviceRepository deviceRepository)
        {
            this._logger = logger;
            this._repository = deviceRepository;
        }
        public async Task<bool> Create(DeviceDto deviceDto)
        {
             var result = true;
            var model = deviceDto.ToModel<DeviceDto, DeviceModel>();

            result = await _repository.Create(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<DeviceDto>> GetAsync()
        {
            var list = await _repository.GetAsync();

            var query = from item in list
                        select item.ToModel<DeviceModel, DeviceDto>();

            return query;
        }

        public async Task<IEnumerable<DeviceDto>> GetAllDevice()
        {
            var list = await _repository.GetAllDevice();
            return list;
        }

        public async Task<IEnumerable<DeviceDto>> GetDeviceGroup()
        {
            var list = await _repository.GetDeviceGroup();

            var query = from item in list
                        select item.ToModel<DeviceModel, DeviceDto>();

            return query;
        }

        public async Task<bool> Update(DeviceDto deviceDto)
        {
            var result = true;
            var model = deviceDto.ToModel<DeviceDto, DeviceModel>();

            result =  await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(DeviceDto deviceDto)
        {
            var result = true;
            var model = deviceDto.ToModel<DeviceDto, DeviceModel>();

            result = await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }
    }
}
