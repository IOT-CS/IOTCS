using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class LoginController
    {

        private readonly IAuthorizationService _service;
        public LoginController()
        {
            _service = IocManager.Instance.GetService<IAuthorizationService>();
        }

        [HttpPost]
        [Route("edge/login")]
        public ActionResult<RestfulData<LoginDto>> Login([FromBody] LoginDto user)
        {           
            var result = new RestfulData<LoginDto>();

            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                result.State_Code = 500;
                result.Data = null;
                result.Message = "用户名或密码错误！";
            }
            else
            {
                var userInfo = _service.GenerateToken(user.UserName, user.Password);
                if (userInfo == null)
                {
                    result.State_Code = 500;
                    result.Data = null;
                    result.Message = "用户名或密码错误！";
                }
                else
                {
                    result.State_Code = 200;
                    result.Data = new LoginDto() 
                    {
                        Token = userInfo.Token,
                        UserName = userInfo.UserName,
                        DisplayName = userInfo.DisplayName
                    };
                    result.Message = "授权成功！";
                }                
            }

            return result;
        }
    }
}
