using IOTCS.EdgeGateway.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Domain.Repositories
{
    public interface IResourceRepository
    {
        Task<bool> Insert(ResourceModel model);

        Task<bool> Update(ResourceModel model);

        Task<bool> Delete(ResourceModel model);

        Task<IEnumerable<ResourceModel>> GetAsync();

        Task<ResourceModel> GetById(string Id);

        Task<bool> TestAsync(ResourceModel model);

    }
}
