using IOTCS.EdgeGateway.Infrastructure.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appSettings.json", true, true);
            })            
            .ConfigureWebHostDefaults(builder => {
                builder.UseStartup<Startup>();
            })
            .UseWindowsService();
    }
}
