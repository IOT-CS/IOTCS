
using IOTCS.EdgeGateway.Freesql;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommonDbContextServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonDbContext(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(ServiceDescriptor.Singleton<ICommonDbSessionContext, CommonDbSessionContext>());

            return services;
        }
    }
}
