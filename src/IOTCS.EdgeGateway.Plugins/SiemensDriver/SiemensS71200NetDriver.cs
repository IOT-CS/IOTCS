using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Library;
using IOTCS.EdgeGateway.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using Volo.Abp.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Plugins.SiemensDriver
{
    public class SiemensS71200NetDriver : ISiemensS71200NetDriver, ISingletonDependency
    {
        private SiemensS7Net _siemensS7NetClient;
        private readonly ILogger _logger;
        private readonly ISystemDiagnostics _diagnostics;
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private IConcurrentList<DataLocationDto> _dataLocations = null;

        public SiemensS71200NetDriver()
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
            var timeout = 0;

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
                        timeout = Convert.ToInt32(djson.Timeout);
                        _siemensS7NetClient = new SiemensS7Net(SiemensPLCS.S1200, host) { Port = port,Slot = djson.slot, ConnectTimeOut = timeout };
                        _siemensS7NetClient.ConnectServer();

                        result = true;
                    }
                    else
                    {
                        var msg = $"Siemens S7 1200 Net 连接失败！失败的Siemens host => {host}，没有找到对应的设备配置信息。";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                var msg = $"Siemens S7 1200 Net 连接失败！失败的Siemens host => {host}，信息 => {e.Message},位置 => {e.StackTrace}";
                _logger.Error(msg);
                _diagnostics.PublishDiagnosticsInfo(msg);
            }

            return result;
        }

        public bool IsAviable()
        {
            return string.IsNullOrEmpty(_siemensS7NetClient.ConnectionId) != true ? true : false;
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
                    switch (d.NodeType)
                    {
                        case "string":
                            var sResult = _siemensS7NetClient.ReadString(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
                            var bitResult = _siemensS7NetClient.ReadBool(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
                            var int16Result = _siemensS7NetClient.ReadInt16(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
                            var uint16Result = _siemensS7NetClient.ReadUInt16(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
                            var int32Result = _siemensS7NetClient.ReadInt32(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
                            var uint32Result = _siemensS7NetClient.ReadUInt32(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
                            var floatResult = _siemensS7NetClient.ReadFloat(d.NodeAddress, Convert.ToUInt16(d.NodeLength));
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
