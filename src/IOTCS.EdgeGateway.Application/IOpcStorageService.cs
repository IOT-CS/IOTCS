using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application
{
    public interface IOpcStorageService
    {
        Task<bool> Insert(string resourceId, string sql);
    }
}
