
using System;

namespace IOTCS.EdgeGateway.Freesql.Helper.Application
{
    public abstract class ServiceBase
    {
        protected ServiceBase(IServiceProvider services)
        {

        }

        public IServiceProvider Services { get; protected set; }
    }
}
