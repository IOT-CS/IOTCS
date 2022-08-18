using IOTCS.EdgeGateway.Freesql.IdleBus;
using System;
using System.Collections.Concurrent;

namespace IOTCS.EdgeGateway.Domain.ValueObject
{
    public class InitailizeDatabaseDto
    {
        public DbBus DBBus { get; set; }

        public ConcurrentDictionary<string, IntPtr> TDengine { get; set; }
    }
}
