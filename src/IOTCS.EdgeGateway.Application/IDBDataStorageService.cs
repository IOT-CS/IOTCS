using IOTCS.EdgeGateway.Domain.ValueObject;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IDBDataStorageService
    {
        Task<bool> Insert(DataRequestDto request);

        Task<bool> BatchInsert(DataRequestDto request);
    }
}
