using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class FreeSqlMgrService : IFreeSqlMgrService
    {        
        private readonly IFreeSqlMgrRepository _repository;

        public FreeSqlMgrService(IFreeSqlMgrRepository repository)
        {
            _repository = repository;            
        }

        public InitailizeDatabaseDto CreateDbConnections(IEnumerable<ResourceDto> resources)
        {
            var models = from r in resources
                         select new ResourceModel 
                         {
                             Id = r.Id,
                             ResourceType = r.ResourceType,
                             ResourceName = r.ResourceName,
                             ResourceParams = r.ResourceParams
                         };
            var database = _repository.CreateDbConnections(models);
            var result = database == null ? null : database.ToModel<InitailizeDatabaseModel, InitailizeDatabaseDto>();

            return result;
        }
    }
}
