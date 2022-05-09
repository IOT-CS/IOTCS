using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Library;
using IOTCS.EdgeGateway.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.OpcUADriver
{
    public class OpcUADriver : IOpcUADriver, ISingletonDependency
    {
        private readonly ILogger _logger;
        private readonly ISystemDiagnostics _diagnostics;
        private OpcUaClient _opcClient = new OpcUaClient();
        private IConcurrentList<DeviceDto> _device = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private IConcurrentList<DataLocationDto> _dataLocations = null;

        public OpcUADriver()
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
            var url = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(deviceID))
                {
                    var config = _deviceConfig.Where(w => w.DeviceId == deviceID).FirstOrDefault();
                    if (config != null && !string.IsNullOrEmpty(config.ConfigJson))
                    {
                        var djson = JsonConvert.DeserializeObject<dynamic>(config.ConfigJson);
                        url = djson.OPCAddr;
                        _opcClient.ConnectServer(url)
                        .ConfigureAwait(false).GetAwaiter().GetResult();
                        result = true;

                    }
                    else
                    {
                        var msg = $"OPC UA 连接失败！失败的OPC URL => {url}，没有找到对应的设备配置信息。";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                var msg = $"OPC UA 连接失败！失败的OPC URL => {url}，信息 => {e.Message},位置 => {e.StackTrace}";
                _logger.Error(msg);
                _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return result;
        }

        public bool IsAviable()
        {
            var result = false;

            result = _opcClient.Connected;

            return result;
        }

        public string Run(string deviceID)
        {
            var result = string.Empty;

            if (_opcClient.Connected)
            {
                var locations = _dataLocations.Where(w => w.ParentId == deviceID);
                var curLocations = from l in locations
                                   select new NodeId(l.NodeAddress);
                var curLocationValues = from l in locations
                                        select new DataNodeDto
                                        {
                                            FieldName = l.DisplayName,
                                            NodeId = l.NodeAddress
                                        };
                List<DataValue> dataValues = _opcClient.ReadNodes(curLocations.ToArray());
                var dataArray = dataValues.ToArray();
                var nodeArray = curLocations.ToArray();
                DataNodeDto dNode = null;
                List<DataNodeDto> list = new List<DataNodeDto>();
                for (var i = 0; i < dataArray.Length; i++)
                {
                    dNode = curLocationValues.Where(w => w.NodeId == nodeArray[i]).FirstOrDefault();
                    dNode.NodeValue = dataArray[i].Value == null ? string.Empty : dataArray[i].Value.ToString();
                    dNode.StatusCode = dataArray[i].StatusCode.Code.ToString();
                    list.Add(dNode);
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
