using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Freesql
{
    public class AppFreeSqlDbContextModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //注册内容到容器中
            var serviceProvider = context.Services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            context.Services.AddCommonDbContext();
            context.Services.AddDbContext(options =>
            {
                options.Database = configuration["ConnectionStrings:DBReadWrite:Database"];
                options.DbIp = configuration["ConnectionStrings:DBReadWrite:DbIp"];
                options.DbPort = configuration["ConnectionStrings:DBReadWrite:DbPort"];
                options.DbType = configuration["ConnectionStrings:DBReadWrite:DbType"];
                options.DbUid = configuration["ConnectionStrings:DBReadWrite:DbUid"];
                options.DbPwd = configuration["ConnectionStrings:DBReadWrite:DbPwd"];
                options.Debug = configuration["Debug"].ToLower() == "true";
            });
        }
    }
}
