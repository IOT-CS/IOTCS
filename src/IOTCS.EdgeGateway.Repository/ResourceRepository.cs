using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Freesql;

namespace IOTCS.EdgeGateway.Repository
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly IFreeSql _freeSql;
        private readonly ICommonDbSessionContext _commonDbSession;

        public ResourceRepository(IFreeSql freeSql, ICommonDbSessionContext sessionContext)
        {
            _freeSql = freeSql;
            _commonDbSession = sessionContext;
        }

        public async Task<bool> Insert(ResourceModel model)
        {
            var currRow = await _freeSql.Select<ResourceModel>()
            .Where(d => d.ResourceName.Equals(model.ResourceName))
            .ToListAsync().ConfigureAwait(false);
            if (currRow != null && currRow.Count > 0)
            {
                return await Task.FromResult<bool>(false);
            }
            var result = false;

            var affrows = _freeSql.Insert(model).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Update(ResourceModel model)
        {
            var currRow = await _freeSql.Select<ResourceModel>()
           .Where(d => d.ResourceName.Equals(model.ResourceName))
           .ToListAsync().ConfigureAwait(false);
            if (currRow == null || currRow.Count == 0)
            {
                return false;
            }
            var result = false;
            var affrows = _freeSql.Update<ResourceModel>().SetSource(model).ExecuteAffrows();
            result = affrows > 0 ? true : false;
            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(ResourceModel model)
        {
            var result = false;
            var drives = await _freeSql.Select<RelationshipModel>().Where(d => d.ResourceId == model.Id).ToListAsync().ConfigureAwait(false);
            if (drives != null && drives.Count > 0)
            {
                return false;
            }
            var affrows =  _freeSql.Delete<ResourceModel>().Where(d => d.Id.Equals(model.Id)).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<ResourceModel>> GetAsync()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<ResourceModel> result = new List<ResourceModel>();

            stringBuilder.Clear();
            stringBuilder.Append("SELECT * FROM tb_resource");

            var query = await _freeSql.Select<ResourceModel>().WithSql(stringBuilder.ToString()).ToListAsync().ConfigureAwait(false);
            if (query != null)
            {
                result.AddRange(query);
            }            

            return result;
        }

        public async Task<bool> TestAsync(ResourceModel model)
        {
            var result = false;
            IFreeSql freeSql = _commonDbSession.CreateDbContext(model, model.ResourceType);
            if (freeSql == null)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return await Task.FromResult(result);
        }

        public async Task<ResourceModel> GetById(string Id)
        {
            ResourceModel result = new ResourceModel();
            var config = await _freeSql.Select<ResourceModel>().Where(d => d.Id == Id).ToListAsync().ConfigureAwait(false);

            if (config != null && config.Count > 0)
            {
                result = config[0];
            }
            return result;
        }
    }
}
