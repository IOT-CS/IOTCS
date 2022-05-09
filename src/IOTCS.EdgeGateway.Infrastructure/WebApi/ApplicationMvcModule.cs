using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.CmdHandler;
using IOTCS.EdgeGateway.Diagnostics.DiagnosticsContext;
using IOTCS.EdgeGateway.Dispatch;
using IOTCS.EdgeGateway.Infrastructure.Extensions;
using IOTCS.EdgeGateway.Infrastructure.Serialize;
using IOTCS.EdgeGateway.Infrastructure.Socket;
using IOTCS.EdgeGateway.Infrastructure.WebApi.Middleware;
using IOTCS.EdgeGateway.MqttHandler;
using IOTCS.EdgeGateway.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    [DependsOn(
        typeof(AppJsonModule),
        typeof(OpcHandlerModule),
        typeof(AbpDiagnosticsContextModule),
        typeof(AbpWebSocketModule),
        typeof(AppServicesModule),
        typeof(AppPluginModule),
        typeof(AppDispatchModule),
        typeof(AppMqttHandlerModule)
        )]
    public class ApplicationMvcModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseCors("CorsPolicy");
            app.UseMiddleware<BasicAuthenticationMiddleware>();
            app.UseAuthentication();
            app.UseApiDoc();
            app.UseMvc(mvcOptions =>
            {
                mvcOptions.MapRoute(name: "default",
                                    template: "{controller=Login}/{action=Post}");
            });
        }
    }
}
