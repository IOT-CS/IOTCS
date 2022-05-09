using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using IOTCS.EdgeGateway.Freesql.IdleBus;

namespace IOTCS.EdgeGateway.Domain.Models
{
    public class InitailizeDatabaseModel
    {
        public DbBus DBBus { get; set; }

        public ConcurrentDictionary<string, IntPtr> TDengine { get; set; }
    }
}
