using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using sim = System;


namespace IOTCS.EdgeGateway.Infrastructure.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddApiDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",        
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "IOTCS.Api",
                        Version = "v1",
                        Description = "ICS de API",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {
                            Name = "IOTCS",
                            Url = new sim.Uri("https://www.iotcs.com.cn/")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense
                        {
                            Name = "www.iotcs.com.cn",
                            Url = new sim.Uri("https://www.iotcs.com.cn/")
                        }                     
                    });
            });
            return services;
        }

        public static IApplicationBuilder UseApiDoc(this IApplicationBuilder app)
        {
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.RoutePrefix = "api-docs";
                   c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1");
                   c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
               });
            return app;
        }
    }
}
