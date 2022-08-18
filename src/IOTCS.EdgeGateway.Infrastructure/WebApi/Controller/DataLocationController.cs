using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Autofac;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class DataLocationController : ControllerBase
    {
        private readonly IDataLocationService _service;
        public DataLocationController()
        {
            _service = IocManager.Instance.GetService<IDataLocationService>();
        }

        [HttpPost]
        [Route("datalocation/data/getAllNodes")]
        public async Task<DataResponseDto<IEnumerable<DataLocationDto>>> GetAsync()
        {
            var result = new DataResponseDto<IEnumerable<DataLocationDto>>();

            var rResult = await _service.GetAsync().ConfigureAwait(false);
            result.Data = rResult;
            result.Successful = true;
            result.ErrorMessage = string.Empty;

            return result;
        }

        [HttpPost]
        [Route("datalocation/data/getNode")]
        public async Task<DataResponseDto<IEnumerable<DataLocationDto>>> GetNodeByIdAsync(DataLocationDto nodelist)
        {
            var result = new DataResponseDto<IEnumerable<DataLocationDto>>();

            var rResult = await _service.GetNodeByIdAsync(nodelist.ParentId).ConfigureAwait(false);
            result.Data = rResult;
            result.Successful = true;
            result.ErrorMessage = string.Empty;
            return result;
        }

        [HttpPost]
        [Route("datalocation/data/insert")]
        public async Task<DataResponseDto<bool>> InsertAsync([FromBody] DataLocationDto nodelist)
        {
            var result = new DataResponseDto<bool>();

            if (nodelist != null)
            {
                var rResult = await _service.Insert(nodelist);
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
        [Route("datalocation/data/delete")]
        public async Task<DataResponseDto<bool>> DeleteAsync([FromBody] DataLocationDto nodelist)
        {
            var result = new DataResponseDto<bool>();

            if (nodelist != null)
            {
                var rResult = await _service.Delete(nodelist);
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
        [Route("datalocation/data/update")]
        public async Task<DataResponseDto<bool>> UpdateAsync([FromBody] DataLocationDto nodelist)
        {
            var result = new DataResponseDto<bool>();

            if (nodelist != null)
            {
                var rResult = await _service.Update(nodelist);
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
    }
}
