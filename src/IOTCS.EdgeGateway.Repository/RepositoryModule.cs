using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Freesql;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Repository
{
    [DependsOn(typeof(AppFreeSqlDbContextModule))]
    public class RepositoryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Add(ServiceDescriptor.Singleton<IUserRepository, UserRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IDataLocationRepository, DataLocationRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IResourceRepository, ResourceRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IFreeSqlMgrRepository, FreeSqlMgrRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IOpcStorageRepository, OpcStorageRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IDeviceRepository, DeviceRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IDriveRepository, DriveRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IDeviceConfigRepository, DeviceConfigRepository>());
            context.Services.Add(ServiceDescriptor.Singleton<IRelationshipRepository, RelationshipRepository>());
        }
    }
}