using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class ResourceDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 资源类型,1.db, 2.http, 3.mqtt
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// 资源参数 => JSON
        /// </summary>
        public string ResourceParams { get; set; }
        public string CreaterBy { get; set; }

        public string CreateTime { get; set; }
    }
}
