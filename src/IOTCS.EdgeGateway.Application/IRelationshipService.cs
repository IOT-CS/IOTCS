using IOTCS.EdgeGateway.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IRelationshipService
    {
        public Task<bool> Create(RelationshipDto deviceDto);

        Task<bool> Update(RelationshipDto deviceModel);

        Task<bool> Delete(RelationshipDto deviceModel);

        Task<IEnumerable<RelationshipDto>> GetAllRelationship();

        Task<IEnumerable<string>> GetTopics();
    }
}
