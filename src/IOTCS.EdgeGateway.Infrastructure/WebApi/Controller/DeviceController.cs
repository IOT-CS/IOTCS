using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _service;

        public DeviceController()
        { 
            _service = IocManager.Instance.GetService<IDeviceService>();
        }

        [HttpPost]
        [Route("device/data/insert")]
        public async Task<DataResponseDto<bool>> Insert([FromHeader(Name = "LoginUser")] string user,[FromBody] DeviceDto deviceDto)
        {
            var result = new DataResponseDto<bool>();

            if (deviceDto != null)
            {
                //创建信息
                deviceDto.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                deviceDto.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Create(deviceDto);
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
        [Route("device/data/update")]
        public async Task<DataResponseDto<bool>> Update([FromHeader(Name = "LoginUser")] string user,[FromBody] DeviceDto deviceDto)
        {
            var result = new DataResponseDto<bool>();

            if (deviceDto != null)
            {
                //创建信息
                deviceDto.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                deviceDto.UpdateBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Update(deviceDto);
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
        [Route("device/data/delete")]
        public async Task<DataResponseDto<bool>> Delete([FromHeader(Name = "LoginUser")] string user,[FromBody] DeviceDto deviceDto)
        {
            var result = new DataResponseDto<bool>();

            if (deviceDto != null)
            {
                //创建信息
                deviceDto.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                deviceDto.UpdateBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Delete(deviceDto);
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
        [Route("device/data/getDeviceGroup")]

        public async Task<DataResponseDto<IEnumerable<DeviceDto>>> GetDeviceGroup()
        {
            var result = new DataResponseDto<IEnumerable<DeviceDto>>();
            var rResult = await _service.GetDeviceGroup();
            result.Successful = true;
            result.Data = rResult;
            return result;
        }

        [HttpGet]
        [Route("device/data/getAllDevice")]
        public async Task<DataResponseDto<IEnumerable<DeviceDto>>> GetAllDevice()
        {
            var result = new DataResponseDto<IEnumerable<DeviceDto>>();
            var devices = await _service.GetAllDevice();
            result.Successful = true;
            result.Data = devices;
            return result;
        }


        [HttpGet]
        [Route("device/data/getDriveNodeType")]
        public async Task<DataResponseDto<string>> GetDriveNodeType(string deviceId)
        {
            var result = new DataResponseDto<string>();
            var nodeTypeJson = await _service.GetNodeTypeConfigByDevId(deviceId);
            result.Successful = true;
            result.Data = nodeTypeJson;
            return result;
        }
    }
}
