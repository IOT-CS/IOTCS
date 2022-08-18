using IOTCS.EdgeGateway.Logging;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddFullLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }
            string debug = configuration["Debug"];
            string value = configuration["LogPath"];
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("日志设置路径为空!");
            }
            Environment.SetEnvironmentVariable("log_home", value);
            if (!string.IsNullOrEmpty(debug) && debug.ToLower() == "true")
            {
                var loggerFactory = new LoggerFactory()
                .AddConsole(LogLevel.Trace, true)
                .AddLog4Net();
                var logger = loggerFactory.CreateLogger("errorAppender");
                services.Add(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));
                services.Add(ServiceDescriptor.Singleton<ILogger>(logger));
            }
            else
            {
                var loggerFactory = new LoggerFactory()
                .AddLog4Net();
                var logger = loggerFactory.CreateLogger("errorAppender");
                services.Add(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));
                services.Add(ServiceDescriptor.Singleton<ILogger>(logger));
            }

            return services;
        }
    }
}
