using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class OpcConfigDto
    {
        public string Id { get; set; }        
        /// <summary>
        /// OPC名称
        /// </summary>
        public string OpcName { get; set; }
        /// <summary>
        /// OPC地址
        /// </summary>
        public string OpcUrl { get; set; }
        /// <summary>
        /// 采集周期 ms
        /// </summary>
        public int Duration { get; set; } = 600;
    }
}
