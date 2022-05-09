using IOTCS.EdgeGateway.Domain.ValueObject;
using System.Collections.Generic;

namespace IOTCS.EdgeGateway.Plugins.Executor
{
    public class ResourceComparer : IEqualityComparer<ResourceDto>
    {
        public bool Equals(ResourceDto x, ResourceDto y)
        {
            if (x == null)
                return y == null;
            return x.ResourceName == y.ResourceName;
        }


        public int GetHashCode(ResourceDto obj)
        {
            if (obj == null)
                return 0;
            return obj.ResourceName.GetHashCode();
        }
    }
}
