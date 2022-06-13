using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Middleware
{
    public sealed class BasicAuthenticationMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly IAuthorizationService _authService;

        public BasicAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;

            _logger = IocManager.Instance.GetService<ILogger>();
            _authService = IocManager.Instance.GetService<IAuthorizationService>();
        }

        public async Task InvokeAsync(HttpContext context)
        {

            await _next.Invoke(context);

            string authentication = context.Request.Headers["XC-Token"];
            var result = new DataResponseDto<string>();
            var path = context.Request.Path.HasValue != true
                ? string.Empty : context.Request.Path.Value;
            if (path.Contains("/edge/login"))
            {
                await _next.Invoke(context);
            }
            else
            {
                if (authentication != null
                    && !string.IsNullOrEmpty(authentication))
                {
                    try
                    {
                        var isPassed = _authService.ValidateToken(authentication);

                        if (isPassed)
                        {
                            await _next.Invoke(context);
                        }
                        else
                        {
                            _logger.Error($"身份验证失败！当前token=>{authentication}");
                            result.Successful = false;
                            result.ErrorMessage = "401 Unauthozied";                            
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.Error($"身份验证失败！当前token=>{authentication}, Msg=>{ex.Message},Stack=>{ex.StackTrace}");                       
                        result.Successful = false;
                        result.ErrorMessage = "401 Unauthozied";                        
                    }
                }
                else
                {
                    _logger.Error($"身份验证失败！当前token=>{authentication}");
                    result.Successful = false;
                    result.ErrorMessage = "401 Unauthozied,请登录后，再调试！";                    
                }
            }

        }
    }
}
