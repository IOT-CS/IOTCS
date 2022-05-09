using IOTCS.EdgeGateway.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IDataLocationRepository
    {
        Task<IEnumerable<DataLocationModel>> GetAsync();
        Task<bool> Insert(DataLocationModel model);
        Task<bool> Update(DataLocationModel model);
        Task<bool> Delete(DataLocationModel model);
        Task<IEnumerable<DataLocationModel>> GetNodeByIdAsync(string id);
    }
}
