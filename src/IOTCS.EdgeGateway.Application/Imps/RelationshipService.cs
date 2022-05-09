using IOTCS.EdgeGateway.Domain.DomainService;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using IOTCS.EdgeGateway.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class RelationshipService : IRelationshipService
    {
        private readonly ILogger _logger;
        private readonly IRelationshipRepository _repository;
        private readonly IRelationshipDomainService _relationshipDomainService;

        public RelationshipService(ILogger logger,
            IRelationshipRepository relationshipRepository,
            IRelationshipDomainService relationshipDomainService)
        {
            this._logger = logger;
            this._repository = relationshipRepository;
            this._relationshipDomainService = relationshipDomainService;
        }

        public async Task<bool> Create(RelationshipDto relationshipDto)
        {
            bool result;
            var model = relationshipDto.ToModel<RelationshipDto, RelationshipModel>();

            result = await _repository.Create(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(RelationshipDto relationshipDto)
        {

            var result = true;
            var model = relationshipDto.ToModel<RelationshipDto, RelationshipModel>();

            result = await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<RelationshipDto>> GetAllRelationship()
        {
            var list = await _relationshipDomainService.GetRelationship();

            return list;
        }
        public async Task<IEnumerable<string>> GetTopics()
        {
            return await _repository.GetTopics();
        }


        public async Task<bool> Update(RelationshipDto relationshipDto)
        {

            var result = true;
            var model = relationshipDto.ToModel<RelationshipDto, RelationshipModel>();

            result = await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }
    }
}
