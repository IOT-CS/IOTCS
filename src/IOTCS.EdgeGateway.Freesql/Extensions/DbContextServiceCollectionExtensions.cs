
using System;
using IOTCS.EdgeGateway.Freesql;
using IOTCS.EdgeGateway.Freesql.Extensions;
using IOTCS.EdgeGateway.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, Action<DbContextOptions> options)
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
            services.Add(ServiceDescriptor.Singleton<IDBSessionContext,DBSessionContext>());            
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger>();
            logger.Info($"初始化FreeSql连接开始............!");
            var dbSession = serviceProvider.GetService<IDBSessionContext>();
            var freeSql = dbSession.CreateDbContext();
            services.Add(ServiceDescriptor.Singleton<IFreeSql>(freeSql));
            logger.Info($"初始化FreeSql连接结束............!");

            return services;
        }
    }
}
