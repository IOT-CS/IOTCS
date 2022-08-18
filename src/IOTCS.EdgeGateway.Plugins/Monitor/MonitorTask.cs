using IOTCS.EdgeGateway.BaseDriver;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.WsHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NewLife;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.Monitor
{
    public class MonitorTask : IMonitorTask, ISingletonDependency
    {
        private readonly ILogger _logger;        
        private WsMessageHandler _webSocket;
        private readonly ISystemDiagnostics _diagnostics;
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<DriveDto> _driver = null;
        private ConcurrentDictionary<string, IDriver> _equipments = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MonitorTask(IHttpContextAccessor httpContextAccessor, WsMessageHandler messageHandler)
        {
            _driver = IocManager.Instance.GetService<IConcurrentList<DriveDto>>();            
            _equipments = IocManager.Instance.GetService<ConcurrentDictionary<string, IDriver>>();
            _device = IocManager.Instance.GetService<IConcurrentList<DeviceDto>>();
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _deviceConfig = IocManager.Instance.GetService<IConcurrentList<DeviceConfigDto>>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            _webSocket = messageHandler;
            _httpContextAccessor = httpContextAccessor;
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

                        var convertedString = JsonConvert.SerializeObject(SystemInfo);
                        //var webSocketString = HandleMessage(convertedString);
                        await Send(convertedString);
                        _logger.Info($"{convertedString}");
                    }
                    catch (Exception e)
                    {
                        var msg = $"Minitor系统异常！，信息 => {e.Message},位置 => {e.StackTrace}";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
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
                await _webSocket.SendMessageToAllAsync(new WebSocketManager.Common.Message
                {
                    MessageType = WebSocketManager.Common.MessageType.Text,
                    RequestType = "dashboard",
                    Data = msg
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.Error($"OPCUA 系统消息发送WebSocket信息到UI异常，异常信息 => {e.Message},位置 => {e.StackTrace}");
            }
        }        
    }
}
