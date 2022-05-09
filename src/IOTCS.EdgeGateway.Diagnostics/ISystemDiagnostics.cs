using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Diagnostics
{
    public interface ISystemDiagnostics
    {
        Task PublishAsync(string message);

        Task PublishDiagnosticsInfoAsync(string message);
    }
}
