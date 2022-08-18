using IOTCS.EdgeGateway.Infrastructure.WebApi;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Infrastructure.Server
{
    [DependsOn(typeof(ApplicationMvcModule))]
    public class ApplicationStartModule : AbpModule
    {
        
    }
}
