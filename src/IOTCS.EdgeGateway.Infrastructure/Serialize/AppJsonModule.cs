using System;
using Newtonsoft.Json;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace IOTCS.EdgeGateway.Infrastructure.Serialize
{
    public class AppJsonModule : AbpModule
    {
        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
			JsonConvert.DefaultSettings = () =>
			{
				var settings = new JsonSerializerSettings();
				// Avoid add items to exist collection.
				settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
				settings.Formatting = Formatting.Indented;
				return settings;
			};
		}
	}
}
