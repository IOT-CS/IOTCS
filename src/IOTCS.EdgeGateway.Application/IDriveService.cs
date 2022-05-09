using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IDriveService
    {
        Task<bool> Create(DriveDto deviceDto);

        Task<IEnumerable<DriveDto>> GetAllrive();

        Task<bool> Update(DriveDto deviceDto);

        Task<bool> Delete(DriveDto deviceDto);
    }
}
