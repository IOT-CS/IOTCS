using IOTCS.EdgeGateway.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IFreeSqlMgrRepository
    {
        InitailizeDatabaseModel CreateDbConnections(IEnumerable<ResourceModel> resources);
    }
}
