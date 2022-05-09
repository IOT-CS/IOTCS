using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Core;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    public class SystemController : ControllerBase
    {
        private SystemManagerDto _publish;

        public SystemController()
        {
            _publish = IocManager.Instance.GetService<SystemManagerDto>();
        }

        [HttpPost]
        [Route("config/data/publish")]
        public async Task<DataResponseDto<bool>> PublishConfigAsync()
        {
            var result = new DataResponseDto<bool>();

            _publish.IsPublishing = true;
            result.Successful = true;
            result.ErrorMessage = string.Empty;

            return await Task.FromResult(result);
        }

        [HttpPost]
        [Route("system/reboot/task")]
        public async Task<DataResponseDto<bool>> RebootTasksAsync()
        {
            var result = new DataResponseDto<bool>();

            _publish.Reboot = true;
            result.Successful = true;
            result.ErrorMessage = string.Empty;

            return await Task.FromResult(result);
        }
    }
}
