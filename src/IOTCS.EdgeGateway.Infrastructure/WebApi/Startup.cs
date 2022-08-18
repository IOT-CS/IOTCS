using IOTCS.EdgeGateway.Infrastructure.Extensions;
using IOTCS.EdgeGateway.Infrastructure.Server;
using IOTCS.EdgeGateway.WebSocketManager;
using IOTCS.EdgeGateway.WsHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    public class Startup
    {
        private IServiceCollection _services = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager(Assembly.GetAssembly(typeof(WsMessageHandler)));
            services.AddHttpContextAccessor();
            services.AddSingleton<IConfiguration>(Configuration);            
            services.AddFullLogging(Configuration);
            services.AddCors(options =>
               options.AddPolicy("CorsPolicy", builder =>
               builder.WithOrigins(
                   Configuration["Application:CorsOrigins"]
                   .Split(",", StringSplitOptions.RemoveEmptyEntries).ToArray()
               ).SetIsOriginAllowedToAllowWildcardSubdomains().WithMethods(new string[] { "POST", "OPTIONS" })
               .AllowAnyHeader()
               )
            );
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // 忽略循环引用
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    // 不使用驼峰
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    // 设置时间格式
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<HttpGlobalExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<Settings>(Configuration.GetSection("Jwt"));
            var Settings = new Settings();
            Configuration.Bind("Jwt", Settings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = Settings.ValidateLifetime,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = Settings.Audience,
                    ValidIssuer = Settings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.SecurityKey))
                };
            });

            services.AddApiDoc();
            _services = services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            _services.AddObjectAccessor<IApplicationBuilder>();
            _services.AddObjectAccessor<IWebHostEnvironment>();
            var serviceProvider = _services.BuildServiceProvider();
            serviceProvider.GetRequiredService<ObjectAccessor<IApplicationBuilder>>().Value = app;
            serviceProvider.GetRequiredService<ObjectAccessor<IWebHostEnvironment>>().Value = environment;
            var application = new ApplicationStartBase(_services);
        }
    }
}
