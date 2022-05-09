using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using IOTCS.EdgeGateway.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.DomainService.Impl
{
    public class RelationshipDomainService : IRelationshipDomainService
    {

        private readonly ILogger _logger;
        private readonly IRelationshipRepository _repository;
        private readonly IResourceRepository _resourceRepository;

        public RelationshipDomainService(ILogger logger, 
            IRelationshipRepository relationshipRepository,
            IResourceRepository resourceRepository)
        {
            this._logger = logger;
            this._repository = relationshipRepository;
            this._resourceRepository = resourceRepository;
        }


        public async Task<IEnumerable<RelationshipDto>> GetRelationship()
        {
            List<RelationshipDto> result = new List<RelationshipDto>();
            //查询Relationship
            var relationships = await _repository.GetRelationship();

            //绑定名称
            foreach (var relationship in relationships)
            {
                var dto = relationship.ToModel<RelationshipModel, RelationshipDto>();

                var resouce = await _resourceRepository.GetById(relationship.ResourceId);
                dto.ResourceName = resouce.ResourceName;
                result.Add(dto);
            }
            return result;
        }
    }
}
