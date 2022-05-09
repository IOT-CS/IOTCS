
using System;

namespace IOTCS.EdgeGateway.Freesql.Helper.Application
{
    public abstract class ServiceBase<TRepository> : ServiceBase
    {
        TRepository _repository;

        protected ServiceBase(IServiceProvider services) : base(services)
        {

        }        

        public TRepository Repository
        {
            get
            {
                if (this._repository == null)
                    this._repository = (TRepository)this.Services.GetService(typeof(TRepository));

                return this._repository;
            }
        }
    }

}
