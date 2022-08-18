using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Diagnostics.DiagnosticsContext
{
    public class AbpDiagnosticsContextModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Add(ServiceDescriptor.Singleton<ISystemDiagnostics, SystemDiagnostics>());
            context.Services.Add(ServiceDescriptor.Singleton<IUINotification, UINotification>());
        }
    }
}
