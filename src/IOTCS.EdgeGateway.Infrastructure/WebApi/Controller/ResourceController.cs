using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Application;
using System;
using Newtonsoft.Json;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class ResourceController: ControllerBase
    {
        private readonly IResourceService _service;
        public ResourceController()
        {
            _service = IocManager.Instance.GetService<IResourceService>();
        }
        
        [HttpPost]
        [Route("resource/data/insert")]
        public async Task<DataResponseDto<bool>> InsertAsync([FromHeader(Name = "LoginUser")] string user, [FromBody] ResourceDto resource)
        {
            var result = new DataResponseDto<bool>();

            if (resource != null)
            {
                //创建信息
                resource.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                resource.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Insert(resource);
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
        [Route("resource/data/get")]
        public async Task<DataResponseDto<IEnumerable<ResourceDto>>> GetAsync()
        {
            var result = new DataResponseDto<IEnumerable<ResourceDto>>();

            var rResult = await _service.GetAsync().ConfigureAwait(false);
            result.Data = rResult;
            result.Successful = true;
            result.ErrorMessage = string.Empty;

            return result;
        }
        [HttpPost]
        [Route("resource/data/delete")]
        public async Task<DataResponseDto<bool>> DeleteAsync([FromBody] ResourceDto resource)
        {
            var result = new DataResponseDto<bool>();

            if (resource != null)
            {
                var rResult = await _service.Delete(resource);
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

        [HttpPost]
        [Route("resource/data/update")]
        public async Task<DataResponseDto<bool>> UpdateAsync([FromHeader(Name = "LoginUser")] string user, [FromBody] ResourceDto resource)
        {
            var result = new DataResponseDto<bool>();

            if (resource != null)
            {
                resource.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                resource.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Update(resource);
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
        [Route("resource/data/test")]
        public async Task<DataResponseDto<bool>> TestAsync([FromBody] ResourceDto resource)
        {
            var result = new DataResponseDto<bool>();

            if (resource != null)
            {
                var rResult = await _service.TestAsync(resource);
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
    }
}
