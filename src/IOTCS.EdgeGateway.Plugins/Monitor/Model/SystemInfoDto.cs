using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Plugins.Monitor
{
    public class SystemInfoDto<T>
    {
        public PerformanceDto Performance { get; set; }

        public List<T> Connections { get; set; }
    }

    public class PerformanceDto
    { 
        public float CpuRate { get; set; }

        public ulong AvailableMemory { get; set; }

        public ulong Memory { get; set; }

        /// <summary>
        /// 网络上行速度
        /// </summary>
        public ulong UplinkSpeed { get; set; }

        /// <summary>
        /// 网络下行速度
        /// </summary>
        public ulong DownlinkSpeed { get; set; }        
    }
}
