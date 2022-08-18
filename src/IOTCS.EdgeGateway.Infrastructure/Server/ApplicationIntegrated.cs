using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Freesql.IdleBus;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.ComResDriver;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.IO;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;
using IOTCS.EdgeGateway.BaseDriver;
using IOTCS.EdgeGateway.Domain.ValueObject.Notification;

namespace IOTCS.EdgeGateway.Infrastructure.Server
{
    public abstract class ApplicationIntegrated<TStartupModule> : ApplicationWithServiceProvider, IDisposable
        where TStartupModule : IAbpModule
    {
        protected IAbpApplication Application { get; }

        protected override IServiceProvider ServiceProvider => Application.ServiceProvider;

        protected IServiceProvider RootServiceProvider { get; }

        protected IServiceScope AppServiceScope { get; }

        protected ApplicationIntegrated(IServiceCollection services)
        {
            ILogger logger = services.BuildServiceProvider().GetService<ILogger>();
            try
            {
                BeforeAddApplication(services);

                var application = services.AddApplication<TStartupModule>(SetApplicationCreationOptions);
                Application = application;

                AfterAddApplication(services);

                RootServiceProvider = CreateServiceProvider(services);
                AppServiceScope = RootServiceProvider.CreateScope();
                IocManager.Instance = RootServiceProvider;
                application.Initialize(AppServiceScope.ServiceProvider);
            }
            catch (Exception e)
            {
                var msg = e.Message + "||" + e.StackTrace;
                logger.Error(msg);
            }
        }

        protected virtual void BeforeAddApplication(IServiceCollection services)
        {
            //初始化缓存对象
            var system = new SystemManagerDto();
            var dbBus = new ConcurrentList<DbBus>();
            var resouces = new ConcurrentList<ResourceDto>();
            var driver = new ConcurrentList<DriveDto>();
            var relationShip = new ConcurrentList<RelationshipDto>();
            var deviceConfig = new ConcurrentList<DeviceConfigDto>();
            var device = new ConcurrentList<DeviceDto>();
            var locations = new ConcurrentList<DataLocationDto>();
            var equipmentDriers = new ConcurrentDictionary<string, IDriver>();
            var resDriver = new ConcurrentDictionary<string, IResourceDriver>();
            var realtimeData = new ConcurrentDictionary<string, NotifyChangeDto>();
            services.AddSingleton<SystemManagerDto>(system);
            services.AddSingleton<IConcurrentList<DbBus>>(dbBus);
            services.AddSingleton<IConcurrentList<ResourceDto>>(resouces);
            services.AddSingleton<IConcurrentList<DriveDto>>(driver);
            services.AddSingleton<IConcurrentList<DeviceConfigDto>>(deviceConfig);
            services.AddSingleton<IConcurrentList<DeviceDto>>(device);
            services.AddSingleton<IConcurrentList<DataLocationDto>>(locations);
            services.AddSingleton<IConcurrentList<RelationshipDto>>(relationShip);
            services.AddSingleton<ConcurrentDictionary<string, NotifyChangeDto>>(realtimeData);
            services.AddSingleton<ConcurrentDictionary<string, IDriver>>(equipmentDriers);
            services.AddSingleton<ConcurrentDictionary<string, IResourceDriver>>(resDriver);
        }

        protected virtual void SetApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
            options.Configuration.BasePath = AppDomain.CurrentDomain.BaseDirectory;
            var plugIns = options.PlugInSources;
            var folderPath = options.Configuration.BasePath + "Plugins";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            plugIns.Add(new FolderPlugInSource(folderPath));
        }

        protected virtual void AfterAddApplication(IServiceCollection services)
        {

        }

        protected virtual IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProviderFromFactory();
        }

        public virtual void Dispose()
        {
            Application.Shutdown();
            AppServiceScope.Dispose();
            Application.Dispose();
        }
    }
}
