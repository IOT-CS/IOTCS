using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.MqttHandler
{
    public class AppMqttHandlerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Add(ServiceDescriptor.Singleton<IMqttSessionContext, MqttSessionContext>());
        }
    }
}
