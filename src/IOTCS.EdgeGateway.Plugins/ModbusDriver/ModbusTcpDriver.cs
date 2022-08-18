using HslCommunication.ModBus;
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
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.ModbusDriver
{
    public class ModbusTcpDriver : IModbusTcpDriver, ISingletonDependency
    {
        private ModbusTcpNet _busTcpClient;
        private readonly ILogger _logger;
        private readonly ISystemDiagnostics _diagnostics;
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private IConcurrentList<DataLocationDto> _dataLocations = null;

        public ModbusTcpDriver()
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
                        _busTcpClient = new ModbusTcpNet(host, port);
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
                List<DataNodeDto> list = new List<DataNodeDto>();
                var locations = _dataLocations.Where(w => w.ParentId == groupID);
                foreach (var d in locations)
                {
                    byte station = 1;
                    string address = string.Empty;
                    if (!string.IsNullOrEmpty(d.NodeAddress) && d.NodeAddress.IndexOf('!') != -1)
                    {
                        var splitArray = d.NodeAddress.Split(new char[] { '!' });
                        station = Convert.ToByte(splitArray[0]);
                        address = splitArray[1];
                    }
                    //并行读取所有数据
                    switch (d.NodeType)
                    {
                        case "string":
                            _busTcpClient.Station = station;
                            var sResult = _busTcpClient.ReadString(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto stringNode = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = sResult.Content,
                                StatusCode = "Good"
                            };
                            list.Add(stringNode);
                            break;
                        case "bit":
                            _busTcpClient.Station = station;
                            var bitResult = _busTcpClient.ReadBool(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto bitNode = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = bitResult.Content.ToString(),
                                StatusCode = "Good"
                            };
                            list.Add(bitNode);
                            break;
                        case "int16":
                            _busTcpClient.Station = station;
                            var int16Result = _busTcpClient.ReadInt16(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto int16Node = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = int16Result.Content.ToString(),
                                StatusCode = "Good"
                            };
                            list.Add(int16Node);
                            break;
                        case "uint16":
                            _busTcpClient.Station = station;
                            var uint16Result = _busTcpClient.ReadUInt16(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto uint16Node = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = uint16Result.Content.ToString(),
                                StatusCode = "Good"
                            };
                            list.Add(uint16Node);
                            break;
                        case "int32":
                            _busTcpClient.Station = station;
                            var int32Result = _busTcpClient.ReadInt32(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto int32Node = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = int32Result.Content.ToString(),
                                StatusCode = "Good"
                            };
                            list.Add(int32Node);
                            break;
                        case "uint32":
                            _busTcpClient.Station = station;
                            var uint32Result = _busTcpClient.ReadUInt32(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto uint32Node = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = uint32Result.Content.ToString(),
                                StatusCode = "Good"
                            };
                            list.Add(uint32Node);
                            break;
                        case "float":
                            _busTcpClient.Station = station;
                            var floatResult = _busTcpClient.ReadFloat(address, Convert.ToUInt16(d.NodeLength));
                            DataNodeDto floatNode = new DataNodeDto
                            {
                                FieldName = d.DisplayName,
                                NodeId = d.NodeAddress,
                                NodeValue = floatResult.Content.ToString(),
                                StatusCode = "Good"
                            };
                            list.Add(floatNode);
                            break;
                    }
                }

                result = JsonConvert.SerializeObject(list);
                list.Clear();
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
