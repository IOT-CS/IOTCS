using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Repository
{
    public class RelationshipRepository : IRelationshipRepository
    {
        private readonly IFreeSql _freeSql;


        public RelationshipRepository(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }
        public async Task<bool> Create(RelationshipModel relationshipModel)
        {
            var currRow = await _freeSql.Select<RelationshipModel>()
                .Where(d=>d.Topic.Equals(relationshipModel.Topic) && d.ResourceId.Equals(relationshipModel.ResourceId))
                .ToListAsync().ConfigureAwait(false);
            if (currRow != null && currRow.Count > 0)
            {
                return false;
            }
            else
            {
                var affrows = _freeSql.Insert(relationshipModel).ExecuteAffrows();
                bool result = affrows > 0;
                return await Task.FromResult<bool>(result);
            }
        }

        public async Task<bool> Delete(RelationshipModel relationshipModel)
        {
            var result = false;

            var affrows = _freeSql.Delete<RelationshipModel>().Where(d => d.Id == relationshipModel.Id).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Update(RelationshipModel relationshipModel)
        {
            var result = false;

            var affrows = _freeSql.Update<RelationshipModel>().SetSource(relationshipModel).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<RelationshipModel>> GetRelationship()
        {
            List<RelationshipModel> result = new List<RelationshipModel>();

            var query = await _freeSql.Select<RelationshipModel>().ToListAsync().ConfigureAwait(false);
            //if (query != null)
            //{
            //    foreach (var model in query)
            //    {
            //       var resourceRet =  await _freeSql.Select<ResourceModel>().Where(d=>d.Id.Equals(model.ResourceId)).ToListAsync().ConfigureAwait(false);
            //        if (resourceRet != null && resourceRet.Count == 1)
            //        {
            //            var dto = model.ToModel<RelationshipModel, RelationshipDto>();
            //            dto.ResourceName = resourceRet[0].ResourceName;
            //            result.Add(dto);
            //        }
            //    }
            //}
            result.AddRange(query);
            return result;
        }

        public async Task<IEnumerable<string>> GetTopics()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Clear();
            stringBuilder.Append("SELECT DISTINCT t.*  from tb_device t where t.DeviceType=1");
            var result =  await _freeSql.Select<DeviceModel>().WithSql(stringBuilder.ToString()).ToListAsync();
            if (result != null)
                return result.Select(d => d.Topic);
            return null;
        }
    }
}
