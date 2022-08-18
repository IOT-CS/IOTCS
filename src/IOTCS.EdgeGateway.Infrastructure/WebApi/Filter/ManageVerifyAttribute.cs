using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Filter
{
    public class ManageVerifyAttribute : Attribute, IActionFilter
    {
        private readonly ILogger _logger;
        //private readonly RequestDelegate _next;
        private readonly IAuthorizationService _authService;

        public ManageVerifyAttribute()
        {
            _logger = IocManager.Instance.GetService<ILogger>();
            _authService = IocManager.Instance.GetService<IAuthorizationService>();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string authentication = context.HttpContext.Request.Headers["XC-Token"];

            if (authentication != null
                    && !string.IsNullOrEmpty(authentication))
            {
                try
                {
                    var isPassed = _authService.ValidateToken(authentication);

                    if (!isPassed)
                    {
                        _logger.Error($"身份验证失败！当前token=>{authentication}");
                        context.Result = new JsonResult($"身份验证失败！当前token=>{authentication}");
                    }
                   
                }
                catch (Exception ex)
                {
                    _logger.Error($"身份验证失败！当前token=>{authentication}, Msg=>{ex.Message},Stack=>{ex.StackTrace}");
                    context.Result = new JsonResult($"身份验证失败！当前token=>{authentication}");
                }
            }
            else
            {
                _logger.Error($"身份验证失败！当前token=>{authentication}");
                context.Result = new JsonResult($"身份验证失败！当前token=>{authentication}");
            }
        }
    }
}
