using IOTCS.EdgeGateway.Domain.Repositories;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class OpcStorageService : IOpcStorageService
    {
        private readonly IOpcStorageRepository _storageRepository;

        public OpcStorageService(IOpcStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }

        public async Task<bool> Insert(string resourceId, string sql)
        {
            var result = false;

            result = await _storageRepository.Insert(resourceId, sql);

            return result;
        }
    }
}
