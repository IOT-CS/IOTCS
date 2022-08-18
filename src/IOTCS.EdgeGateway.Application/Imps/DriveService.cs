using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Freesql.Helper;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class DriveService : IDriveService
    {

        private readonly ILogger _logger;
        private readonly IDriveRepository _repository;

        public DriveService(ILogger logger,
                           IDriveRepository driveRepository)
        {
            _logger = logger;
            _repository = driveRepository;
        }
        public async Task<bool> Create(DriveDto driveDto)
        {
            var result = true;
            var model = driveDto.ToModel<DriveDto, DriveModel>();

            result = await _repository.Create(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<DriveDto>> GetAllrive()
        {
            var list = await _repository.GetAllDrive();

            var query = from item in list
                        select item.ToModel<DriveModel, DriveDto>();

            return query;
        }

        public async Task<bool> Update(DriveDto driveDto)
        {
            var result = true;
            var model = driveDto.ToModel<DriveDto, DriveModel>();

            result = await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(DriveDto driveDto)
        {
            var result = true;
            var model = driveDto.ToModel<DriveDto, DriveModel>();

            result = await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }
    }
}
