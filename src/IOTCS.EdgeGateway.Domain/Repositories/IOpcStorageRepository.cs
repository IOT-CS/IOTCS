using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IOpcStorageRepository
    {
        Task<bool> Insert(string resourceId, string sql);

        Task<bool> BatchInsert(string resourceId, IEnumerable<string> sqlList);
    }
}
