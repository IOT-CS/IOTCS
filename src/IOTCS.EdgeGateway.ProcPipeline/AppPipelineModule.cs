using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using IOTCS.EdgeGateway.BaseProcPipeline;

namespace IOTCS.EdgeGateway.ProcPipeline
{
    public class AppPipelineModule : AbpModule
    {
        //public override void ConfigureServices(ServiceConfigurationContext context)
        //{
        //    var pipeline = new PipelineContext();
        //    context.Services.Add(ServiceDescriptor.Singleton<IPipelineContext>(pipeline));
        //}

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {           
            context.Services.Add(ServiceDescriptor.Singleton<IPipelineContext, PipelineContext>());
        }
    }
}
