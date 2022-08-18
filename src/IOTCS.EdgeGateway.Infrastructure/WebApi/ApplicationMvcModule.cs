using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.CmdHandler;
using IOTCS.EdgeGateway.Diagnostics.DiagnosticsContext;
using IOTCS.EdgeGateway.Infrastructure.Extensions;
using IOTCS.EdgeGateway.Infrastructure.Serialize;
using IOTCS.EdgeGateway.Infrastructure.WebApi.Middleware;
using IOTCS.EdgeGateway.MqttHandler;
using IOTCS.EdgeGateway.Plugins;
using IOTCS.EdgeGateway.ProcPipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp;
using Volo.Abp.Modularity;
using IOTCS.EdgeGateway.WsHandler;
using IOTCS.EdgeGateway.WebSocketManager;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    [DependsOn(
        typeof(AppJsonModule),
        typeof(OpcHandlerModule),
        typeof(AbpDiagnosticsContextModule),        
        typeof(AppServicesModule),
        typeof(AppPluginModule),
        typeof(AppPipelineModule),
        typeof(AppMqttHandlerModule)
        )]
    public class ApplicationMvcModule : AbpModule
    {
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var serviceProvider = context.ServiceProvider;
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

            app.UseWebSockets(new WebSocketOptions {
                KeepAliveInterval = TimeSpan.FromDays(30)
            });
            app.MapWebSocketManager("/ws", serviceProvider.GetService<WsMessageHandler>());
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
