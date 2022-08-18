using IOTCS.EdgeGateway.Domain.DomainService;
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
        private readonly IDeviceDomainService _deviceDomianService;
        private readonly IDriveRepository _driveRepository;


        public DeviceService(IServiceProvider services
            , ILogger logger
            , IDeviceRepository deviceRepository
            , IDeviceDomainService deviceDomainService
            , IDriveRepository driveRepository)
        {
            this._logger = logger;
            this._repository = deviceRepository;
            this._deviceDomianService = deviceDomainService;
            this._driveRepository = driveRepository;
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

            return await _deviceDomianService.GetAllDevice();
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

            result = await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(DeviceDto deviceDto)
        {
            var result = true;
            var model = deviceDto.ToModel<DeviceDto, DeviceModel>();

            result = await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<string> GetNodeTypeConfigByDevId(string deviceId)
        {
            //获取驱动
            var drive = await _driveRepository.GetDriveByGroupId(deviceId);

            var ret = await _repository.GetNodeTypeConfigByDriveType(drive.DriveType);

            return ret;
        }
    }
}
