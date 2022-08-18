using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Core.Security;
using IOTCS.EdgeGateway.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController()
        {
            _service = IocManager.Instance.GetService<IUserService>();
        }

        [HttpPost]
        [Route("user/data/insert")]
        public async Task<DataResponseDto<bool>> Insert([FromHeader(Name = "LoginUser")] string user, [FromBody] UserDto userDto)
        {
            var result = new DataResponseDto<bool>();

            if (userDto != null)
            {
                if (userDto.UserName.ToLower().Equals("admin"))
                {
                    result.Successful = false;
                    result.ErrorMessage = "不可创建admin用户！";
                }
                else
                {
                    //创建信息
                    userDto.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    userDto.Creator = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                    userDto.Password = MD5Helper.GenerateMd5String(userDto.Password);
                    var rResult = await _service.Insert(userDto);
                    result.Successful = rResult;
                    result.ErrorMessage = rResult ? "" : "创建失败,请检查类型及是否重复!";
                }
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }

        [HttpPost]
        [Route("user/data/delete")]
        public async Task<DataResponseDto<bool>> Delete([FromBody] UserDto userDto)
        {
            var result = new DataResponseDto<bool>();

            if (userDto != null)
            {
                //删除信息
                if (userDto.UserName.ToLower().Equals("admin"))
                {
                    result.Successful = false;
                    result.ErrorMessage = "不可删除admin用户！";
                }
                else
                {
                    var rResult = await _service.Delete(userDto);
                    result.Successful = rResult;
                    result.ErrorMessage = rResult ? "" : "删除失败,请检查类型及是否重复!";
                }
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }


        [HttpPost]
        [Route("user/data/changepwd")]
        public async Task<DataResponseDto<bool>> ChangePwd([FromHeader(Name = "LoginUser")] string user, [FromBody] UserDto deviceDto)
        {
            var result = new DataResponseDto<bool>();

            if (deviceDto != null)
            {
                //创建信息
                deviceDto.UpdatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                deviceDto.Updator = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.UpdatePwd(deviceDto);
                result.Successful = rResult;
                result.ErrorMessage = rResult ? "" : "更新失败,检查密码是否正确！";
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }

        [HttpGet]
        [Route("user/data/getUsers")]
        public async Task<DataResponseDto<IEnumerable<UserDto>>> GetAllDevice()
        {
            var result = new DataResponseDto<IEnumerable<UserDto>>();
            var users = await _service.GetAsync();
            result.Successful = true;
            result.Data = users;
            return result;
        }


    }
}
