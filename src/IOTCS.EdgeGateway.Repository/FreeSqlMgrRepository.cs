using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Freesql.IdleBus;
using IOTCS.EdgeGateway.Freesql;

namespace IOTCS.EdgeGateway.Repository
{
    public class FreeSqlMgrRepository : IFreeSqlMgrRepository
    {
        private readonly ICommonDbSessionContext _commonDbSession;

        public FreeSqlMgrRepository(ICommonDbSessionContext sessionContext)
        {
            _commonDbSession = sessionContext;
        }

        public InitailizeDatabaseModel CreateDbConnections(IEnumerable<ResourceModel> resources)
        {
            var database = new InitailizeDatabaseModel();
            database.DBBus = new DbBus();
            database.TDengine = new ConcurrentDictionary<string, IntPtr>();
            //var tdengine = new TDengineHelper();

            foreach (var r in resources)
            {
                switch (r.ResourceType)
                {
                    case "TDengine":
                        //if (!database.TDengine.ContainsKey(r.Id))
                        //{
                        //    var connector = tdengine.TDConnection(r);
                        //    database.TDengine.TryAdd(r.Id, connector);
                        //}
                        break;
                    default:
                        database.DBBus.Register(r.Id, () => {
                            IFreeSql freeSql = _commonDbSession.CreateDbContext(r, r.ResourceType);
                            return freeSql;
                        });
                        break;
                }
            }

            return database;
        }
    }
}
