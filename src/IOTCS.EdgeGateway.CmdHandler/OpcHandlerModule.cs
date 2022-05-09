using MediatR;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.CmdHandler
{
    public class OpcHandlerModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMediatR(new[] { typeof(OpcUaNotificationHandler), typeof(UINotificationHandler), typeof(SystemNotificationHandler) });
        }
    }
}
