using System;
using Microsoft.Extensions.DependencyInjection;

namespace IOTCS.EdgeGateway.Infrastructure.Server
{
    public abstract class ApplicationWithServiceProvider
    {
        protected abstract IServiceProvider ServiceProvider { get; }

        protected virtual T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        protected virtual T GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
