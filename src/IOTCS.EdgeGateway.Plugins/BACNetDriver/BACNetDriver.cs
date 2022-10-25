using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using Microsoft.Extensions.DependencyInjection;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using IOTCS.EdgeGateway.Logging;
using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace IOTCS.EdgeGateway.Plugins.BACNetDriver
{
    public class BACNetDriver : IBACNetDriver
    {
        private readonly ILogger _logger;
        private readonly ISystemDiagnostics _diagnostics;
        private int _bacnetDeviceId = 0;
        private BacnetClient _bacneClient;
        private BacnetAddress _bacneAddr = null;
        private IConcurrentList<DeviceConfigDto> _deviceConfig = null;
        private IConcurrentList<DataLocationDto> _dataLocations = null;
        public BACNetDriver()
        {
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            _deviceConfig = IocManager.Instance.GetService<IConcurrentList<DeviceConfigDto>>();
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _dataLocations = IocManager.Instance.GetService<IConcurrentList<DataLocationDto>>();
        }
        public bool Connect(string deviceID)
        {
            var result = false;
            try
            {
                if (!string.IsNullOrEmpty(deviceID))
                {
                    //获取配置
                    var config = _deviceConfig.Where(w => w.DeviceId == deviceID).FirstOrDefault();
                    if (config != null && !string.IsNullOrEmpty(config.ConfigJson))
                    {
                        var djson = JsonConvert.DeserializeObject<dynamic>(config.ConfigJson);
                        _bacnetDeviceId = Convert.ToInt32(djson.Host);
                        if (Convert.ToInt32(djson.Port) != 0)
                            _bacneClient = new BacnetClient(new BacnetIpUdpProtocolTransport(Convert.ToInt32(djson.Port), false));
                        else//使用默认端口
                            _bacneClient = new BacnetClient(new BacnetIpUdpProtocolTransport(0xBAC0, false));
                        _bacneClient.Start();
                        _bacneClient.OnIam += new BacnetClient.IamHandler(Handler_OnIam);
                        _bacneClient.WhoIs();
                    }
                    else
                    {
                        var msg = $"BACNet连接失败！设备ID => {deviceID}，没有找到对应的设备配置信息。";
                        _logger.Error(msg);
                        _diagnostics.PublishDiagnosticsInfo(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                var msg = $"BACNet连接失败！设备ID => {deviceID}，信息 => {ex.Message},位置 => {ex.StackTrace}";
                _logger.Error(msg);
                _diagnostics.PublishDiagnosticsInfo(msg);
            }
            return result;
        }

        public bool IsAviable()
        {
            return _bacneAddr != null;
        }

        public string Run(string deviceID, string groupID)
        {
            var result = string.Empty;
            if (IsAviable())
            {
                //var nodeValues = new Dictionary<string, BacnetValue>();
                var locations = _dataLocations.Where(w => w.ParentId == groupID);
                var curLocationValues = from l in locations
                                        select new DataNodeDto
                                        {
                                            FieldName = l.DisplayName,
                                            NodeId = l.NodeAddress
                                        };
                List<DataNodeDto> list = new List<DataNodeDto>();
                DataNodeDto dNode = null;
                foreach (var l in locations)
                {
                    BacnetValue currNodeValue;
                    dNode = curLocationValues.Where(w => w.NodeId == l.Id).FirstOrDefault();
                    var ObjName = Regex.Replace(l.NodeAddress, "[a-z]", "", RegexOptions.IgnoreCase);
                    var ObjAddr = Convert.ToUInt32(Regex.Replace(l.NodeAddress, "[0-9]", "", RegexOptions.IgnoreCase));
                    var readRet = ReadScalarValue(new BacnetObjectId(ConvertBacnetType(ObjName), ObjAddr), BacnetPropertyIds.PROP_PRESENT_VALUE, out currNodeValue);
                    dNode.NodeValue = (string)currNodeValue.Value;
                    dNode.StatusCode = readRet ? "good" : "error";
                    list.Add(dNode);
                    // nodeValues.Add(l.Id, currNodeValue);
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
        private void Handler_OnIam(BacnetClient sender, BacnetAddress adr, uint device_id, uint max_apdu, BacnetSegmentations segmentation, ushort vendor_id)
        {
            if (device_id == _bacnetDeviceId)
                _bacneAddr = adr;
        }

        private bool ReadScalarValue(BacnetObjectId BacnetObjet, BacnetPropertyIds Propriete, out BacnetValue Value)
        {
            IList<BacnetValue> NoScalarValue;

            Value = new BacnetValue(null);

            // Property Read
            if (_bacneClient.ReadPropertyRequest(_bacneAddr, BacnetObjet, Propriete, out NoScalarValue) == false)
                return false;

            Value = NoScalarValue[0];
            return true;
        }

        private BacnetObjectTypes ConvertBacnetType(string objName)
        {
            if (objName.Equals("AI"))
                return BacnetObjectTypes.OBJECT_ANALOG_INPUT;
            if (objName.Equals("AO"))
                return BacnetObjectTypes.OBJECT_ANALOG_OUTPUT;
            if (objName.Equals("AV"))
                return BacnetObjectTypes.OBJECT_ANALOG_VALUE;
            if (objName.Equals("BI"))
                return BacnetObjectTypes.OBJECT_BINARY_INPUT;
            if (objName.Equals("BO"))
                return BacnetObjectTypes.OBJECT_BINARY_OUTPUT;
            if (objName.Equals("BV"))
                return BacnetObjectTypes.OBJECT_BINARY_VALUE;
            if (objName.Equals("MSI"))
                return BacnetObjectTypes.OBJECT_MULTI_STATE_INPUT;
            if (objName.Equals("MSO"))
                return BacnetObjectTypes.OBJECT_MULTI_STATE_OUTPUT;
            if (objName.Equals("MSV"))
                return BacnetObjectTypes.OBJECT_MULTI_STATE_VALUE;
            return BacnetObjectTypes.OBJECT_ANALOG_INPUT;
        }
    }
}
