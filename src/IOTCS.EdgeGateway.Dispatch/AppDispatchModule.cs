using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Dispatch
{
    public class AppDispatchModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Add(ServiceDescriptor.Singleton<IDispatchManager, DispatchManager>());
        }
    }
}
