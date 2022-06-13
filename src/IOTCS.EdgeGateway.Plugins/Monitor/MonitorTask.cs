using IOTCS.EdgeGateway.BaseDriver;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.DotNetty;
using IOTCS.EdgeGateway.Logging;
using Microsoft.Extensions.DependencyInjection;
using NewLife;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.Monitor
{
    public class MonitorTask : IMonitorTask, ISingletonDependency
    {
        private readonly ILogger _logger;
        private readonly IWebSocketServer _webSocket;
        private readonly ISystemDiagnostics _diagnostics;
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<DriveDto> _driver = null;
        private ConcurrentDictionary<string, IDriver> _equipments = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;

        public MonitorTask()
        {
            _driver = IocManager.Instance.GetService<IConcurrentList<DriveDto>>();
            _webSocket = IocManager.Instance.GetService<IWebSocketServer>();
            _equipments = IocManager.Instance.GetService<ConcurrentDictionary<string, IDriver>>();
            _device = IocManager.Instance.GetService<IConcurrentList<DeviceDto>>();
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _deviceConfig = IocManager.Instance.GetService<IConcurrentList<DeviceConfigDto>>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
        }

        public bool Executing()
        {
            MachineInfo.RegisterAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        var SystemInfo = new SystemInfoDto<OpcConnectionStatusDto>();
                        SystemInfo.Performance = new PerformanceDto
                        {
                            CpuRate = MachineInfo.Current.CpuRate,
                            Memory = MachineInfo.Current.Memory,
                            AvailableMemory = MachineInfo.Current.AvailableMemory,
                            UplinkSpeed = MachineInfo.Current.UplinkSpeed,
                            DownlinkSpeed = MachineInfo.Current.DownlinkSpeed
                        };

                        SystemInfo.Connections = new List<OpcConnectionStatusDto>();
                        if (_equipments != null)
                        {
                            foreach (var client in _equipments)
                            {
                                var config = _deviceConfig.Where(w => w.DeviceId == client.Key).FirstOrDefault();
                                var device = _device.Where(w => w.Id == client.Key).FirstOrDefault();
                                var driver = _driver.Where(w => w.Id == device.DriveId).FirstOrDefault();
                                if (config != null && driver != null && !string.IsNullOrEmpty(config.ConfigJson))
                                {
                                    var djson = JsonConvert.DeserializeObject<dynamic>(config.ConfigJson);
                                    SystemInfo.Connections.Add(new OpcConnectionStatusDto
                                    {
                                        OpcName = driver.DriveName,
                                        OpcUrl = djson.OPCAddr,
                                        IsConnected = client.Value == null ? false : client.Value.IsAviable(),
                                        ConnectTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))
                                    });
                                }
                            }
                        }

                        var convertedString = JsonConvert.SerializeObject(SystemInfo);
                        var webSocketString = HandleMessage(convertedString);
                        await Send(webSocketString).ConfigureAwait(false);
                        _logger.Info($"{convertedString}");
                    }
                    catch (Exception e)
                    {
                        var msg = $"Minitor系统异常！，信息 => {e.Message},位置 => {e.StackTrace}";
                        _logger.Error(msg);
                        await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                    }

                    //10s 一个周期
                    await Task.Delay(10000);
                }
            });

            return true;
        }

        private async Task Send(string msg)
        {
            try
            {
                var connections = _webSocket?.GetAllConnections();
                if (connections != null && connections.Count > 0)
                {
                    foreach (var conn in connections)
                    {
                        await conn.Send(msg);

                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"OPCUA 系统消息发送WebSocket信息到UI异常，异常信息 => {e.Message},位置 => {e.StackTrace}");
            }
        }

        private string HandleMessage(string sendMessage)
        {
            var result = string.Empty;
            var socketObject = new WebSocketProtocol<string>();

            if (!string.IsNullOrEmpty(sendMessage))
            {
                socketObject.RequestType = "dashboard";
                socketObject.Data = sendMessage;
                result = JsonConvert.SerializeObject(socketObject);
            }

            return result;
        }
    }
}
