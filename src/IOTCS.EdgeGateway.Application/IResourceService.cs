using IOTCS.EdgeGateway.Domain.ValueObject;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IResourceService
    {
        Task<IEnumerable<ResourceDto>> GetAsync();

        Task<bool> Insert(ResourceDto data);

        Task<bool> Update(ResourceDto data);
        Task<bool> Delete(ResourceDto data);

        Task<bool> TestAsync(ResourceDto data);
    }
}
