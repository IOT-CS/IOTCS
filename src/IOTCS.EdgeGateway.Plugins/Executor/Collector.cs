using IOTCS.EdgeGateway.BaseDriver;
using IOTCS.EdgeGateway.BaseProcPipeline;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Plugins.DataInitialize;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.Executor
{
    public class Collector : ICollector, ISingletonDependency
    {
        //private int usingResource = 0;
        private SystemManagerDto _system;
        private IPipelineContext _actor;
        private readonly IInitializeConfiguration _initialize;
        private readonly ILogger _logger;
        private readonly ISystemDiagnostics _diagnostics;
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private IConcurrentList<DriveDto> _driver = null;
        private IConcurrentList<DeviceDto> _device = null;
        private List<Task> _tasks = new List<Task>();
        private ConcurrentDictionary<string, IDriver> _equipments = null;

        public Collector()
        {
            _actor = IocManager.Instance.GetService<IPipelineContext>();
            _system = IocManager.Instance.GetService<SystemManagerDto>();            
            _initialize = IocManager.Instance.GetService<IInitializeConfiguration>();
            _driver = IocManager.Instance.GetService<IConcurrentList<DriveDto>>();
            _device = IocManager.Instance.GetService<IConcurrentList<DeviceDto>>();
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _deviceConfig = IocManager.Instance.GetService<IConcurrentList<DeviceConfigDto>>();
            _equipments = IocManager.Instance.GetService<ConcurrentDictionary<string, IDriver>>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
        }

        public void Run()
        {
            LoadDriver();
            CreateCollectingTasks();
            StartRebootTask();
        }

        public void StartRebootTask()
        {
            var token = new CancellationToken();
            Task task = new Task(async () => {
                while (true)
                {
                    try
                    {
                        ReloadConfig();
                    }
                    catch (Exception e)
                    {
                        var msg = $"重启任务失败！，信息 => {e.Message},位置 => {e.StackTrace}";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
                    }

                    await Task.Delay(5000);
                }
            }, token, TaskCreationOptions.LongRunning);
            task.Start();
        }

        private void ReloadConfig()
        {
            if (_system.IsPublishing)
            {
                _logger.Error($"配置数据重新加载开始");
                _initialize.Executing();
                _system.IsPublishing = false;
                _logger.Error($"配置数据重新加载结束");
            }
        }

        private void CreateCollectingTasks()
        {
            //deviceType = 0 表示设备，deviceType = 1 表示是分组
            var groups = _device.Where(w => w.DeviceType == "0" && w.InUse == 1);
            _tasks.Clear();
            foreach (var device in groups)
            {
                Task task = new Task(() => {

                    try
                    {
                        var poller = new NetMQPoller();
                        var groupList = device.Childrens == null ? new List<DeviceDto>()
                        : device.Childrens.OrderBy(o => o.Duration).ToList<DeviceDto>();
                        foreach (var g in groupList)
                        {
                            _logger.Info($"GroupName => {g.DeviceName},duration => {g.Duration}");
                            var timer = new NetMQTimer(TimeSpan.FromMilliseconds(g.Duration));
                            timer.Elapsed += (s, e) => {
                                if (!_system.IsPublishing)
                                {
                                    CollectingData(device.Id, g.Id);
                                }
                            };
                            poller.Add(timer);
                        }

                        poller.Run();
                    }
                    catch (Exception e)
                    {
                        var msg = $"任务执行失败！，信息 => {e.Message},位置 => {e.StackTrace}";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
                    }                   
                }, _tokenSource.Token, TaskCreationOptions.LongRunning);
                _tasks.Add(task);
                task.Start();
            }
        }       

        //改造
        private void CollectingData(string deviceID, string groupID)
        {
            try
            {
                if (_equipments.ContainsKey(deviceID))
                {
                    var driver = _equipments[deviceID];
                    var messgae = driver.Run(deviceID, groupID);
                    if (!string.IsNullOrEmpty(messgae))
                    {
                        var sendingMsg = BuildingJsonObject(messgae, deviceID, groupID);           
                        _actor.SendPayload(new RouterMessage { Message = sendingMsg, OriginMessage = messgae });                                        
                    }
                    else
                    {
                        var msg = $"当前驱动采集数据为空。驱动ID号=>{deviceID}!";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
                    }
                }
                else
                {
                    var msg = $"当前采集设备没有匹配到驱动。驱动ID号=>{deviceID}!";
                    _logger.Error(msg);
                    _diagnostics.PublishDiagnosticsInfo(msg);
                }
            }
            catch (Exception e)
            {
                var msg = $"当前设备数据采集失败！，信息 => {e.Message},位置 => {e.StackTrace}";
                _logger.Error(msg);
                _diagnostics.PublishDiagnosticsInfo(msg);
            }
        }

        private void LoadDriver()
        {
            //deviceType = 0 表示设备，deviceType = 1 表示是分组
            var equipments = _device.Where(w => w.DeviceType == "0" && w.InUse == 1);

            foreach (var e in equipments)
            {
                var driver = _driver.Where(w => w.Id == e.DriveId).FirstOrDefault();
                if (driver != null)
                {
                    switch (driver.DriveType.ToLower())
                    {
                        case "opcua":
                            var opcua = new OpcUADriver.OpcUADriver();
                            opcua.Connect(e.Id);
                            _equipments.TryAdd(e.Id, opcua);
                            break;
                        case "modbus-tcp":
                            var modbus_tcp = new ModbusDriver.ModbusTcpDriver();
                            modbus_tcp.Connect(e.Id);
                            _equipments.TryAdd(e.Id, modbus_tcp);
                            break;
                        case "siemens-s7-1200":
                            var siemens_s7_1200 = new SiemensDriver.SiemensS71200NetDriver();
                            siemens_s7_1200.Connect(e.Id);
                            _equipments.TryAdd(e.Id, siemens_s7_1200);
                            break;
                        case "siemens-s7-1500":
                            var siemens_s7_1500 = new SiemensDriver.SiemensS71500NetDriver();
                            siemens_s7_1500.Connect(e.Id);
                            _equipments.TryAdd(e.Id, siemens_s7_1500);
                            break;
                    }
                }
                else
                {
                    var msg = $"当前驱动不存在，驱动ID号=>{e.DriveId}";
                    _logger.Info(msg);
                    _diagnostics.PublishDiagnosticsInfo(msg);
                }
            }
        }

        private string BuildingJsonObject(string data, string deviceID, string groupID)
        {
            //var output = string.Empty;
            var result = string.Empty;
            var retResult = new List<dynamic>();
            IEnumerable<DataNodeDto> list = JsonConvert.DeserializeObject<IEnumerable<DataNodeDto>>(data);
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            try
            {  
                var device = _device.Where(w => w.Id == deviceID).FirstOrDefault();
                var group = device.Childrens.Where(w => w.Id == groupID).FirstOrDefault();
                keyValues.Clear();
                keyValues.Add("GroupName", group.DeviceName);
                keyValues.Add("Topic", group.Topic);
                keyValues.Add("DeviceID", deviceID);
                keyValues.Add("GroupID", groupID);
                keyValues.Add("Timestamp", DateTime.Now.ToString());
                foreach (var e in list)  
                {
                    keyValues.Add(e.FieldName, e.NodeValue);
                }
                result = JsonConvert.SerializeObject(keyValues);
            }
            catch (Exception ex)
            {
                _logger.Error($"ExecutorTask => 解析成JSON数据报错=> 位置 => {ex.Message},异常 => {ex.StackTrace}");
            }

            return result;
        }
    }
}
