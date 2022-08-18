using IOTCS.EdgeGateway.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<bool> Insert(UserModel model);

        Task<bool> UpdatePwd(UserModel model);
        Task<bool> Delete(UserModel model);

        Task<IEnumerable<UserModel>> GetAsync();

        Task<UserModel> GetInfoByUserName(UserModel model);
    }
}
