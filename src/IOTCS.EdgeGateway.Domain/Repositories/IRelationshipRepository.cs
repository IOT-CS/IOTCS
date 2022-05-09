using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IRelationshipRepository
    {
        Task<bool> Create(RelationshipModel deviceModel);

        Task<IEnumerable<RelationshipModel>> GetRelationship();

        Task<bool> Delete(RelationshipModel deviceModel);

        Task<bool> Update(RelationshipModel relationshipModel);

        Task<IEnumerable<string>> GetTopics();
    }
}
