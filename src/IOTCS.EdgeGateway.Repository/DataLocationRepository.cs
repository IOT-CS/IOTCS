using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Repository
{
    public class DataLocationRepository : IDataLocationRepository
    {
        private readonly IFreeSql _freeSql;

        public DataLocationRepository(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }
        public async Task<bool> Insert(DataLocationModel model)
        {
            var currRow = await _freeSql.Select<DataLocationModel>()
             .Where(d => d.ParentId.Equals(model.ParentId) && d.DisplayName.Equals(model.DisplayName))
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

        public async Task<bool> Delete(DataLocationModel model)
        {

            var result = false;

            var affrows = _freeSql.Delete<DataLocationModel>(model).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }
        public async Task<bool> Update(DataLocationModel model)
        {
            var currRow = await _freeSql.Select<DataLocationModel>()
             .Where(d => d.ParentId.Equals(model.ParentId) && d.NodeAddress.Equals(model.NodeAddress))
             .ToListAsync().ConfigureAwait(false);
            if (currRow != null && currRow.Count > 1)
            {
                return false;
            }
            var result = false;

            var affrows = _freeSql.Update<DataLocationModel>().SetSource(model).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }
        public async Task<IEnumerable<DataLocationModel>> GetAsync()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<DataLocationModel> result = new List<DataLocationModel>();

            stringBuilder.Clear();
            stringBuilder.Append("SELECT * FROM tb_datalocation");

            var query = await _freeSql.Select<DataLocationModel>().WithSql(stringBuilder.ToString()).ToListAsync().ConfigureAwait(false);
            if (query != null)
            {
                result.AddRange(query);
            }

            return result;
        }
        public async Task<IEnumerable<DataLocationModel>> GetNodeByIdAsync(string id)
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<DataLocationModel> result = new List<DataLocationModel>();

            stringBuilder.Clear();
            stringBuilder.Append("SELECT * FROM tb_datalocation where Parentid='" + id + "'");

            var query = await _freeSql.Select<DataLocationModel>().WithSql(stringBuilder.ToString()).ToListAsync().ConfigureAwait(false);
            if (query != null && query.Count > 0)
                result.AddRange(query);
            return result;
        }
    }
}
