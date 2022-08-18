using Microsoft.Extensions.DependencyInjection;

namespace IOTCS.EdgeGateway.Infrastructure.Server
{
    public class ApplicationStartBase : ApplicationIntegrated<ApplicationStartModule>
    {
        public ApplicationStartBase(IServiceCollection services) : base(services)
        { }
    }
}
