using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using Newtonsoft.Json;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class DriveController: ControllerBase
    {
        private readonly IDriveService _service;


        public DriveController()
        {
            _service = IocManager.Instance.GetService<IDriveService>();
        }

        [HttpPost]
        [Route("drive/data/insert")]
        public async Task<DataResponseDto<bool>> Insert([FromHeader(Name = "LoginUser")] string user,[FromBody] DriveDto driveDto)
        {
            var result = new DataResponseDto<bool>();

            if (driveDto != null)
            {
                //创建信息
                driveDto.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                driveDto.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Create(driveDto);
                result.Successful = rResult;
                result.ErrorMessage = rResult ? "" : "创建失败,请检查类型及是否重复!";
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }

        [HttpPost]
        [Route("drive/data/update")]
        public async Task<DataResponseDto<bool>> Update([FromHeader(Name = "LoginUser")] string user,[FromBody] DriveDto driveDto)
        {
            var result = new DataResponseDto<bool>();

            if (driveDto != null)
            {
                //创建信息
                driveDto.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                driveDto.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Update(driveDto);
                result.Successful = rResult;
                result.ErrorMessage = rResult ? "" : "更新失败,请检查类型及是否重复!";
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }

        [HttpPost]
        [Route("drive/data/delete")]
        public async Task<DataResponseDto<bool>> Delete([FromBody] DriveDto driveDto)
        {
            var result = new DataResponseDto<bool>();

            if (driveDto != null)
            {
                var rResult = await _service.Delete(driveDto);
                result.Successful = rResult;
                result.ErrorMessage = string.Empty;
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }


        [HttpGet]
        [Route("drive/data/getAllDrive")]
        public async Task<DataResponseDto<IEnumerable<DriveDto>>> GetAllDevice()
        {
            var result = new DataResponseDto<IEnumerable<DriveDto>>();
            var devices = await _service.GetAllrive();
            result.Successful = true;
            result.Data = devices;
            return result;
        }





    }
}
