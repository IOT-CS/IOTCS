using IOTCS.EdgeGateway.Application.Imps;
using IOTCS.EdgeGateway.Domain.DomainService;
using IOTCS.EdgeGateway.Domain.DomainService.Impl;
using IOTCS.EdgeGateway.Repository;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Application
{
    [DependsOn(typeof(RepositoryModule))]
    public class AppServicesModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.Add(ServiceDescriptor.Singleton<IUserService, UserService>());
            context.Services.Add(ServiceDescriptor.Singleton<IAuthorizationService, AuthorizationService>());
            context.Services.Add(ServiceDescriptor.Singleton<IDataLocationService, DataLocationService>());
            context.Services.Add(ServiceDescriptor.Singleton<IDeviceService, DeviceService>());
            context.Services.Add(ServiceDescriptor.Singleton<IDriveService, DriveService>());
            context.Services.Add(ServiceDescriptor.Singleton<IDeviceConfigService, DeviceConfigService>());
            context.Services.Add(ServiceDescriptor.Singleton<IFreeSqlMgrService, FreeSqlMgrService>());
            context.Services.Add(ServiceDescriptor.Singleton<IOpcStorageService, OpcStorageService>());
            context.Services.Add(ServiceDescriptor.Singleton<IResourceService, ResourceService>());
            context.Services.Add(ServiceDescriptor.Singleton<IDBDataStorageService, DBDataStorageService>());
            context.Services.Add(ServiceDescriptor.Singleton<IRelationshipService, RelationshipService>());
            context.Services.Add(ServiceDescriptor.Singleton<IRelationshipDomainService, RelationshipDomainService>());
            context.Services.Add(ServiceDescriptor.Singleton<IDeviceDomainService, DeviceDomainService>());
        }
    }
}
