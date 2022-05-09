using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Freesql.IdleBus;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.ComResDriver;
using IOTCS.EdgeGateway.ResDriver;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using IOTCS.EdgeGateway.Domain.ValueObject.Notification;

namespace IOTCS.EdgeGateway.Plugins.DataInitialize
{
    public class InitializeConfiguration : IInitializeConfiguration, ISingletonDependency
    {
        private readonly ILogger _logger;
        private readonly IFreeSqlMgrService _freeSql;
        private readonly ISystemDiagnostics _diagnostics;
        private IConcurrentList<DbBus> _dbBuses = null;
        private IConcurrentList<DataLocationDto> _dataLocations = null;
        private IConcurrentList<DriveDto> _driver = null;
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<ResourceDto> _resources = null;
        private IConcurrentList<RelationshipDto> _relationships = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private ConcurrentDictionary<string, NotifyChangeDto> _keyValues = null;
        private ConcurrentDictionary<string, IntPtr> _dbConnectors = null;
        private ConcurrentDictionary<string, IResourceDriver> _resourceDriver = null;
        private readonly IDeviceService _deviceService;
        private readonly IDriveService _driveService;
        private readonly IRelationshipService _relationshipService;
        private readonly IDataLocationService _dataLocationService;
        private readonly IDeviceConfigService _deviceConfigService;
        private readonly IResourceService _resourceService;

        public InitializeConfiguration()
        {
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _freeSql = IocManager.Instance.GetService<IFreeSqlMgrService>();
            _dbBuses = IocManager.Instance.GetService<IConcurrentList<DbBus>>();
            _resources = IocManager.Instance.GetService<IConcurrentList<ResourceDto>>();
            _dataLocations = IocManager.Instance.GetService<IConcurrentList<DataLocationDto>>();
            _driver = IocManager.Instance.GetService<IConcurrentList<DriveDto>>();
            _device = IocManager.Instance.GetService<IConcurrentList<DeviceDto>>();
            _relationships = IocManager.Instance.GetService<IConcurrentList<RelationshipDto>>();
            _dbConnectors = IocManager.Instance.GetService<ConcurrentDictionary<string, IntPtr>>();
            _deviceConfig = IocManager.Instance.GetService<IConcurrentList<DeviceConfigDto>>();
            _dataLocationService = IocManager.Instance.GetService<IDataLocationService>();
            _deviceService = IocManager.Instance.GetService<IDeviceService>();
            _driveService = IocManager.Instance.GetService<IDriveService>();
            _deviceConfigService = IocManager.Instance.GetService<IDeviceConfigService>();
            _resourceService = IocManager.Instance.GetService<IResourceService>();
            _relationshipService = IocManager.Instance.GetService<IRelationshipService>();
            _keyValues = IocManager.Instance.GetService<ConcurrentDictionary<string, NotifyChangeDto>>();
            _resourceDriver = IocManager.Instance.GetService<ConcurrentDictionary<string, IResourceDriver>>();
        }

        public void Executing()
        {
            var msg = $"配置数据初始化开始.....!";
            _logger.Info(msg);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {
                Parallel.Invoke(
                    async () => { await GetDriverAsync(); },
                    async () => { await GetDevicesAsync(); },
                    async () => { await GetDeviceConfigAsync(); },
                    async () => { await GetResourceAsync(); },
                    async () => { await GetLocationsAsync(); },
                    async () => { await GetRelationShipAsync(); }
                    );
            }
            catch (Exception e)
            {
                var eMsg = $"初始化OPC UA 配置数据失败! 错误信息=> {e.Message}, 错误位置=>{e.StackTrace}";
                _logger.Error(eMsg);
                _diagnostics.PublishDiagnosticsInfoAsync(eMsg).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            watch.Stop();
            long useTime = watch.ElapsedMilliseconds;
            msg = $"配置数据初始化结束, 用时=>{(useTime / 1000.0)}秒!";
            _logger.Info(msg);
        }

        /// <summary>
        /// 加载设备驱动数据
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetLocationsAsync()
        {
            var result = true;

            var retDataLocations = await _dataLocationService.GetAsync().ConfigureAwait(false);
            if (retDataLocations != null && retDataLocations.Count() > 0)
            {
                _dataLocations.Clear();
                foreach (var location in retDataLocations)
                {
                    _dataLocations.Add(location);
                }
            }

            return result;
        }

        /// <summary>
        /// 加载设备分组数据
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetDevicesAsync()
        {
            var result = true;

            var devices = await _deviceService.GetAsync().ConfigureAwait(false);
            if (devices != null && devices.Count() > 0)
            {
                _device.Clear();
                foreach (var device in devices)
                {
                    _device.Add(device);
                }
            }

            return result;
        }

        /// <summary>
        /// 加载设备配置数据
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetDeviceConfigAsync()
        {
            var result = true;

            var deviceConfig = await _deviceConfigService.GetAsync().ConfigureAwait(false);
            if (deviceConfig != null && deviceConfig.Count() > 0)
            {
                _deviceConfig.Clear();
                foreach (var conf in deviceConfig)
                {
                    _deviceConfig.Add(conf);
                }
            }

            return result;
        }

        private async Task<bool> GetDriverAsync()
        {
            var result = true;

            var driver = await _driveService.GetAllrive().ConfigureAwait(false);
            if (driver != null && driver.Count() > 0)
            {
                _driver.Clear();
                foreach (var d in driver)
                {
                    _driver.Add(d);
                }
            }

            return result;
        }

        private async Task<bool> GetResourceAsync()
        {
            var result = true;

            var resources = await _resourceService.GetAsync().ConfigureAwait(false);
            if (resources != null && resources.Count() > 0)
            {
                _keyValues.Clear();
                _resources.Clear();
                foreach (var r in resources)
                {
                    _resources.Add(r);
                    switch (r.ResourceType.ToLower())
                    {
                        case "mqtt":
                            var mqttDriver = new MqttDriver();
                            mqttDriver.Initialize(r.ResourceParams);
                            _resourceDriver.TryAdd(r.Id, mqttDriver);
                            break;
                        case "webhook":
                            var httpDriver = new HttpDriver();
                            httpDriver.Initialize(r.ResourceParams);
                            _resourceDriver.TryAdd(r.Id, httpDriver);
                            break;
                    }
                }
            }

            return result;
        }

        private async Task<bool> GetRelationShipAsync()
        {
            var result = true;

            var relationShip = await _relationshipService.GetAllRelationship().ConfigureAwait(false);
            if (relationShip != null && relationShip.Count() > 0)
            {
                _relationships.Clear();
                foreach (var r in relationShip)
                {
                    _relationships.Add(r);
                }
            }

            return result;
        }
    }
}
