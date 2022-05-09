using IOTCS.EdgeGateway.ComResDriver;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Collections;
using IOTCS.EdgeGateway.Diagnostics;
using IOTCS.EdgeGateway.Domain.ValueObject;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Dispatch
{
    public class DispatchManager : IDispatchManager
    {
        private readonly ILogger _logger;
        private IConcurrentList<RelationshipDto> relationships;
        private ConcurrentDictionary<string, IResourceDriver> resDrivers;
        private readonly ISystemDiagnostics _diagnostics;
        public DispatchManager()
        {
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            relationships = IocManager.Instance.GetService<IConcurrentList<RelationshipDto>>();
            resDrivers = IocManager.Instance.GetService<ConcurrentDictionary<string, IResourceDriver>>();
        }

        public async Task RunTaskAsync(dynamic data)
        {
            var topic = Convert.ToString(data.Topic);
            var reslationship = relationships.Where(d => d.Topic.Equals(topic)).ToList();
            if (relationships != null && relationships.Count > 0)
            {
                foreach (var res in reslationship)
                {
                    if (resDrivers.ContainsKey(res.ResourceId))
                    {
                        var driver = resDrivers[res.ResourceId];
                        driver.Run(data);
                    }
                    else
                    {
                        var msg = $"数据处理异常！Topic => {topic},未找到资源驱动！数据已舍弃";
                        await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
                    }
                }
            }
            else
            {
                var msg = $"数据处理异常！Topic => {topic},未找到关联资源！数据已舍弃";
                await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
            }
        }
    }
}
