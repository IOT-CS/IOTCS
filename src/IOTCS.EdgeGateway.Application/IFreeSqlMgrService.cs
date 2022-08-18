using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using IOTCS.EdgeGateway.Domain.ValueObject;

namespace IOTCS.EdgeGateway.Application
{
    public interface IFreeSqlMgrService
    {
        InitailizeDatabaseDto CreateDbConnections(IEnumerable<ResourceDto> resources);
    }
}
