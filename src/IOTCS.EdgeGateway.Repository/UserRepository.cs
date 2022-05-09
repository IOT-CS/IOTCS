using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.Models;

namespace IOTCS.EdgeGateway.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IFreeSql _freeSql;

        public UserRepository(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        public async Task<bool> Insert(UserModel model)
        {
            var currRow = await _freeSql.Select<UserModel>()
           .Where(d => d.UserName.Equals(model.UserName))
           .ToListAsync().ConfigureAwait(false);
            if (currRow != null && currRow.Count > 0)
            {
                return false;
            }
            var result = false;

            var affrows = _freeSql.Insert(model).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> UpdatePwd(UserModel model)
        {
            var currRow = await _freeSql.Select<UserModel>()
           .Where(d => d.UserName.Equals(model.UserName))
           .ToListAsync().ConfigureAwait(false);
            if (currRow == null || currRow.Count == 0)
            {
                return false;
            }
            var result = false;

            //密码验证
            var affrows =   _freeSql.Update<UserModel>(1) .Set(a => a.Password, model.Password)
                            .Where(a => a.UserName == model.UserName)
                             .ExecuteAffrows();
            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(UserModel model)
        {
            var result = false;

            var affrows = _freeSql.Delete<UserModel>().Where(d => d.Id == model.Id).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<UserModel>> GetAsync()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<UserModel> result = new List<UserModel>();

            stringBuilder.Clear();
            stringBuilder.Append("SELECT * FROM tb_users");

            var query = await _freeSql.Select<UserModel>().WithSql(stringBuilder.ToString()).ToListAsync().ConfigureAwait(false);
            if (query != null)
            {
                result.AddRange(query);
            }

            return result;
        }

        public async Task<UserModel> GetInfoByUserName(UserModel model)
        {
            var user = await _freeSql.Select<UserModel>().Where(d => d.UserName == model.UserName).ToOneAsync().ConfigureAwait(false);

            return user;
        }
    }
}
