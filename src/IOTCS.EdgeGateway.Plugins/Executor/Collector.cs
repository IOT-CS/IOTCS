using IOTCS.EdgeGateway.BaseDriver;
using IOTCS.EdgeGateway.Commands;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Plugins.DataInitialize;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
        private IMediator _mediator;
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
            _system = IocManager.Instance.GetService<SystemManagerDto>();
            _mediator = IocManager.Instance.GetService<IMediator>();
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
                        await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
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

            var groups = _device.Where(w => w.DeviceType == "0");
            _tasks.Clear();
            foreach (var group in groups)
            {
                //var config = _deviceConfig.Where(w => w.DeviceId == deviceID).FirstOrDefault();
                Task task = new Task(async () => {
                    while (!_tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            if (_system.IsPublishing)
                            {
                                var msg = $"正在加载配置数据........!";
                                _logger.Error(msg);
                                await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                                await Task.Delay(1000);
                                continue;
                            }
                            var list = _device.Where(w => w.ParentId == group.Id);
                            CollectingData(list);

                            await Task.Delay(1000);
                        }
                        catch (Exception e)
                        {
                            var msg = $"任务执行失败！，信息 => {e.Message},位置 => {e.StackTrace}";
                            _logger.Error(msg);
                            await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                        }
                    }
                }, _tokenSource.Token, TaskCreationOptions.LongRunning);
                _tasks.Add(task);
                task.Start();
            }
        }

        private void CollectingData(IEnumerable<DeviceDto> list)
        {
            var cts = new CancellationTokenSource();
            Parallel.ForEach(list, new ParallelOptions
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = TaskScheduler.Default
            },
            async (d, state) =>
            {
                try
                {
                    if (_equipments.ContainsKey(d.Id))
                    {
                        var driver = _equipments[d.Id];
                        var messgae = driver.Run(d.Id);
                        if (!string.IsNullOrEmpty(messgae))
                        {
                            var sendingMsg = BuildingJsonObject(messgae, d.Id);
                            //发送订阅数据到界面
                            var subString = sendingMsg.Payload.Values.FirstOrDefault() == null
                            ? string.Empty : JsonConvert.SerializeObject(sendingMsg.Payload.Values.FirstOrDefault());
                            await _diagnostics.PublishAsync(subString).ConfigureAwait(false);
                            await _mediator.Publish<UINotification>(new UINotification { UIMessage = messgae, DeviceID = d.Id }).ConfigureAwait(false);
                            //每一个OPC,PLC设备数据单独发走
                            await _mediator.Publish<OpcUaNotification>(new OpcUaNotification { Message = JsonConvert.SerializeObject(sendingMsg) }).ConfigureAwait(false);
                        }
                        else 
                        {
                            var msg = $"当前驱动采集数据为空。驱动ID号=>{d.Id}, 设备类型 => {d.DeviceType}!";
                            _logger.Error(msg);
                            await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                        }
                      
                    }
                    else
                    {
                        var msg = $"当前采集设备没有匹配到驱动。驱动ID号=>{d.Id}, 设备类型 => {d.DeviceType}!";
                        _logger.Error(msg);
                        await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    var msg = $"当前设备数据采集失败！，信息 => {e.Message},位置 => {e.StackTrace}";
                    _logger.Error(msg);
                    await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                }
            });
        }

        private void LoadDriver()
        {
            var equipments = _device.Where(w => w.DeviceType == "1" && w.InUse == 1);

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
                    }
                }
                else
                {
                    var msg = $"当前驱动不存在，驱动ID号=>{e.DriveId}";
                    _logger.Info(msg);
                    _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
        }

        private SendMessageDto BuildingJsonObject(string data, string deviceID)
        {
            var result = string.Empty;
            SendMessageDto payload = null;
            var retResult = new List<dynamic>();
            IEnumerable<DataNodeDto> list = JsonConvert.DeserializeObject<IEnumerable<DataNodeDto>>(data);
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            try
            {
                var device = _device.Where(w => w.Id == deviceID).FirstOrDefault();
                var group = _device.Where(w => w.Id == device.ParentId).FirstOrDefault();
                keyValues.Clear();
                keyValues.Add("GroupName", group.DeviceName);
                keyValues.Add("Topic", device.Topic);
                keyValues.Add("DeviceID", deviceID);
                keyValues.Add("Ts", DateTime.Now.ToString());
                foreach (var e in list)  
                {
                    keyValues.Add(e.FieldName, e.NodeValue);
                }
                result = JsonConvert.SerializeObject(keyValues);
                retResult.Add(JsonConvert.DeserializeObject<dynamic>(result));

                var message = new PayloadMessage
                {
                    Values = retResult
                };
                payload = new SendMessageDto { Payload = message };
            }
            catch (Exception ex)
            {
                _logger.Error($"ExecutorTask => 解析成JSON数据报错=> 位置 => {ex.Message},异常 => {ex.StackTrace}");
            }

            return payload;
        }
    }
}
