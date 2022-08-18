using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Diagnostics
{
    public interface ISystemDiagnostics
    {
        void Publish(string message);

        void PublishDiagnosticsInfo(string message);
    }
}
