using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Device;
using Newtonsoft.Json;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class DeviceConfigController : ControllerBase
    {
        private readonly IDeviceConfigService _service;


        public DeviceConfigController()
        {
            _service = IocManager.Instance.GetService<IDeviceConfigService>();
        }


        [HttpPost]
        [Route("device/config/getDeviceConfig")]

        public async Task<DataResponseDto<DeviceConfigDto>> GetDeviceGroup([FromBody] DeviceConfigDto data)
        {
            var result = new DataResponseDto<DeviceConfigDto>();
            var rResult = await _service.GetAllDeviceConfigByDeviceId(data.DeviceId);
            result.Successful = true;
            result.Data = rResult;
            return result;
        }

        [HttpPost]
        [Route("device/config/insert")]
        public async Task<DataResponseDto<bool>> Insert([FromHeader(Name = "LoginUser")] string user,[FromBody] DeviceConfigDto data)
        {
            var result = new DataResponseDto<bool>();
            data.CreateBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
            data.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var rResult = await _service.Create(data);
            result.Successful = true;
            result.Data = rResult;
            return result;
        }

        [HttpPost]
        [Route("device/config/update")]
        public async Task<DataResponseDto<bool>> Update([FromHeader(Name = "LoginUser")] string user,[FromBody] DeviceConfigDto data)
        {
            var result = new DataResponseDto<bool>();
            data.UpdateBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
            data.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var rResult = await _service.Update(data);
            result.Successful = true;
            result.Data = rResult;
            return result;
        }
    }
}
