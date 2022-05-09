using IOTCS.EdgeGateway.Domain.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IDataLocationService
    {
        Task<IEnumerable<DataLocationDto>> GetAsync();

        Task<IEnumerable<DataLocationDto>> GetNodeByIdAsync(string id);

        Task<bool> Insert(DataLocationDto data);

        Task<bool> Update(DataLocationDto data);
        Task<bool> Delete(DataLocationDto data);
    }
}
