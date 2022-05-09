using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using IOTCS.EdgeGateway.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class UserService : IUserService
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _repository;

        public UserService(IServiceProvider services
            , ILogger logger
            , IUserRepository repository)
            
        {
            this._logger = logger;
            this._repository = repository;            
        }

        public async Task<UserDto> GetUserInfoByName(string userName)
        {
            var user = await _repository.GetInfoByUserName(new UserModel { UserName = userName }).ConfigureAwait(false);
            var result = user == null ? null : user.ToModel<UserModel, UserDto>();

            return result;
        }

        public async Task<IEnumerable<UserDto>> GetAsync()
        {
            var list = await _repository.GetAsync();

            var query = from s in list
                        select s.ToModel<UserModel, UserDto>();

            return query;
        }

        public async Task<bool> Insert(UserDto data)
        {
            var result = true;
            var model = data.ToModel<UserDto, UserModel>();

            var rResult = _repository.Insert(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> UpdatePwd(UserDto data)
        {
            var result = false;
            var model = data.ToModel<UserDto, UserModel>();

            var currUser = await _repository.GetInfoByUserName(model);
            if (currUser.Password.Equals(data.OldPassword))
            {
                result =  await _repository.UpdatePwd(model);
            }

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(UserDto data)
        {
            var result = true;
            var model = data.ToModel<UserDto, UserModel>();

            await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }
    }
}
