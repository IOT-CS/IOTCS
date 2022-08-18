using IOTCS.EdgeGateway.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Net;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        public HttpGlobalExceptionFilter(ILoggerFactory logger, IWebHostEnvironment env)
        {
            _logger = logger.CreateLogger("SystemWebApi");
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            if(!context.ExceptionHandled)
            {
                var msg = context.Exception.Message + "||" + context.Exception.StackTrace;
                _logger.Error(msg);              
                context.ExceptionHandled = true;
            }
        }
    }

    public class ApplicationErrorResult : ObjectResult
    {
        public ApplicationErrorResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
