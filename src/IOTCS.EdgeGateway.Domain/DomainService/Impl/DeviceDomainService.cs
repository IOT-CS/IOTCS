using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.DomainService.Impl
{
    public class DeviceDomainService : IDeviceDomainService
    {

        private readonly IDeviceRepository _repository;
        private readonly IDriveRepository _driveRepository;

        public DeviceDomainService(
            IDeviceRepository deviceRepository,
            IDriveRepository driveRepository)
        {
            this._repository = deviceRepository;
            this._driveRepository = driveRepository;
        }


        public async Task<IEnumerable<DeviceDto>> GetAllDevice()
        {
            var result = new List<DeviceDto>();
            //查询设备
            var devices = await _repository.GetDeviceGroup();

            var devicesDto = from item in devices
                             select item.ToModel<DeviceModel, DeviceDto>();
            foreach (var device in devicesDto)
            {
                //赋值Group
                var groups = await _repository.GetGroupByDeviceId(device.Id);
                var groupsDto = from item in groups
                                select item.ToModel<DeviceModel, DeviceDto>();
                device.Childrens = groupsDto.ToList();

                //赋值驱动
                var drive = await _driveRepository.GetDriveByDeviceId(device.Id);

                device.DriveName = drive.DriveName;
                result.Add(device);
            }
            return result;
        }
    }
}
