using IOTCS.EdgeGateway.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IDriveRepository
    {
        Task<bool> Create(DriveModel deviceModel);


        Task<IEnumerable<DriveModel>> GetAllDrive();

        Task<bool> Update(DriveModel deviceModel);

        Task<bool> Delete(DriveModel deviceModel);
    }
}
