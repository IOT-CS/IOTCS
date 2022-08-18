using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class SystemManagerDto
    {
        public bool IsPublishing { get; set; } = false;

        public bool Reboot { get; set; } = false;
    }
}
