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
using HslCommunication.ModBus;
using Volo.Abp.DependencyInjection;

namespace IOTCS.EdgeGateway.Plugins.ModbusDriver
{
    public class ModbusRTUDriver : IModbusRTUDriver, ISingletonDependency
    {
        public bool Connect(string deviceID)
        {
            throw new NotImplementedException();
        }

        public bool IsAviable()
        {
            throw new NotImplementedException();
        }

        public string Run(string deviceID, string groupID)
        {
            throw new NotImplementedException();
        }
    }
}
