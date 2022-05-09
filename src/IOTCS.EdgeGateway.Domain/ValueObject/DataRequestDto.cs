using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class DataRequestDto
    {
        /// <summary>
        /// 数据资源ID号
        /// </summary>
        public string ResourceId { get; set; }

        public string InputMessage { get; set; }

        public string SqlTemplate { get; set; }
    }
}
