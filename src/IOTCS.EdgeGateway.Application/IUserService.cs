using IOTCS.EdgeGateway.Domain.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IUserService
    {
        Task<UserDto> GetUserInfoByName(string userName);
        Task<IEnumerable<UserDto>> GetAsync();
        Task<bool> Insert(UserDto data);
        Task<bool> UpdatePwd(UserDto data);
        Task<bool> Delete(UserDto data);
    }
}
