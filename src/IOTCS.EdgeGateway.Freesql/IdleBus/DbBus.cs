using System;

namespace IOTCS.EdgeGateway.Freesql.IdleBus
{    
    public class DbBus : IdleBus<string, IFreeSql>
    {
        public DbBus() : base(TimeSpan.FromMinutes(30)) { }
    }
}
