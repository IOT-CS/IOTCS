using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Freesql.IdleBus;
using IOTCS.EdgeGateway.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Repository
{
    public class OpcStorageRepository : IOpcStorageRepository
    {
        private readonly ILogger _logger;
        private IConcurrentList<DbBus> _dbBuses = null;

        public OpcStorageRepository(IConcurrentList<DbBus> dbBuses, ILogger logger)
        {
            this._logger = logger;
            this._dbBuses = dbBuses;
        }

        public async Task<bool> Insert(string resourceId ,string sql)
        {
            var result = false;
            var freeSql = _dbBuses.FirstOrDefault();
            IFreeSql dbFreeSql = freeSql.Get(resourceId);
            if (dbFreeSql != null)
            {
                var affrows = await dbFreeSql.Ado.ExecuteNonQueryAsync(sql)
                .ConfigureAwait(false);
                result = affrows > 0 ? true : false;
                _logger.Info($"Executing Result = {result},执行的SQL ={sql},资源ID = {resourceId}");
            }
            else
            {
                _logger.Info($"没有找到对应的数据库资源,资源ID = {resourceId}");
            }           

            return result;
        }

        public async Task<bool> BatchInsert(string resourceId, IEnumerable<string> sqlList)
        {
            var result = true;
            var tempBool = false;

            var freeSql = _dbBuses.FirstOrDefault();
            foreach (var sql in sqlList)
            {
                var affrows = await freeSql.Get(resourceId).Ado.ExecuteNonQueryAsync(sql);
                tempBool = affrows > 0 ? true : false;
                result = result && tempBool;
            }

            return result;
        }
    }
}
