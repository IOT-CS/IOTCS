using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using HslCommunication;
using HslCommunication.ModBus;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.ModbusDriver
{
    public class ModbusRTUOverTcpDriver : IModbusRTUOverTcpDriver, ISingletonDependency
    {
        private ModbusRtuOverTcp _busTcpClient;
        private readonly ILogger _logger;
        private readonly ISystemDiagnostics _diagnostics;
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private IConcurrentList<DataLocationDto> _dataLocations = null;

        public ModbusRTUOverTcpDriver()
        {
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            _device = IocManager.Instance.GetService<IConcurrentList<DeviceDto>>();
            _deviceConfig = IocManager.Instance.GetService<IConcurrentList<DeviceConfigDto>>();
            _dataLocations = IocManager.Instance.GetService<IConcurrentList<DataLocationDto>>();
        }

        public bool Connect(string deviceID)
        {
            var result = false;
            var host = string.Empty;
            var port = 0;

            try
            {
                if (!string.IsNullOrEmpty(deviceID))
                {
                    var config = _deviceConfig.Where(w => w.DeviceId == deviceID).FirstOrDefault();
                    if (config != null && !string.IsNullOrEmpty(config.ConfigJson))
                    {
                        var djson = JsonConvert.DeserializeObject<dynamic>(config.ConfigJson);
                        host = djson.Host;
                        port = Convert.ToInt32(djson.Port);
                        _busTcpClient = new ModbusRtuOverTcp(host, port);
                        _busTcpClient.ConnectServer();

                        result = true;
                    }
                    else
                    {
                        var msg = $"Modbus tcp 连接失败！失败的modbus host => {host}，没有找到对应的设备配置信息。";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                var msg = $"Modbus tcp 连接失败！失败的modbus host => {host}，信息 => {e.Message},位置 => {e.StackTrace}";
                _logger.Error(msg);
                _diagnostics.PublishDiagnosticsInfo(msg);
            }

            return result;
        }

        public bool IsAviable()
        {
            var result = false;

            result = string.IsNullOrEmpty(_busTcpClient.ConnectionId) ? false : true;

            return result;
        }

        public string Run(string deviceID, string groupID)
        {
            var result = string.Empty;

            if (IsAviable())
            {
                var locations = _dataLocations.Where(w => w.ParentId == groupID);

            }
            else
            {
                //设备重连
                Connect(deviceID);
            }

            return result;
        }
    }
}
