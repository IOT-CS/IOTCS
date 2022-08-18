using System.Collections.Generic;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    public class OpenApiData
    {
        public int State_Code { get; set; } = 200;

        public string Message { get; set; }

    }
    public class RestfulData<T> : OpenApiData
    {
        public virtual T Data { get; set; }
        
    }

    public class RestfulArray<T> : RestfulData<IEnumerable<T>>
    {

    }
}
