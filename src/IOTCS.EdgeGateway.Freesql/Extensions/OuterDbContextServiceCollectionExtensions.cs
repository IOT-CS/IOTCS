
using IOTCS.EdgeGateway.Freesql;
using IOTCS.EdgeGateway.Freesql.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OuterDbContextServiceCollectionExtensions
    {
        public static IServiceCollection AddOuterDbContext(this IServiceCollection services, Action<OuterDbContextOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.AddOptions();
            services.Configure(options);
            services.Add(ServiceDescriptor.Singleton<IOuterDBSessionContext, OuterDBSessionContext>());

            return services;
        }
    }
}
