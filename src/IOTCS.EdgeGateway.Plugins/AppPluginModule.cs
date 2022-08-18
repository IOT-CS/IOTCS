using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Plugins.Executor;
using IOTCS.EdgeGateway.Plugins.Monitor;
using IOTCS.EdgeGateway.Plugins.DataInitialize;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Plugins
{
    public class AppPluginModule : AbpModule
    {
        /// <summary>
        /// 插件运行启动函数，同时也是插件操作的入口
        /// </summary>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var serviceProvider = context.ServiceProvider;
            var logger = serviceProvider.GetService<ILogger>();
            try
            {
                var init = serviceProvider.GetService<IInitializeConfiguration>();
                init.Executing();
                var executor = serviceProvider.GetService<ICollector>();
                executor.Run();
                var monitorTask = serviceProvider.GetService<IMonitorTask>();
                monitorTask.Executing();
            }
            catch (Exception e)
            {
                logger.Error($"插件启动异常, 信息 => {e.Message},位置 => {e.StackTrace}");
            }
        }
    }
}
