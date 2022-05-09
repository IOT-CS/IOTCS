using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IOTCS.EdgeGateway.Application;
using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Domain.ValueObject;

namespace IOTCS.EdgeGateway.Infrastructure.WebApi.Controller
{
    [ApiController]
    public class RelationshipController : ControllerBase
    {
        private readonly IRelationshipService _service;

        public RelationshipController()
        {
            _service = IocManager.Instance.GetService<IRelationshipService>();        
        }

        [HttpPost]
        [Route("rule/relationship/get")]
        public async Task<DataResponseDto<IEnumerable<RelationshipDto>>> GetAsync()
        {
            var result = new DataResponseDto<IEnumerable<RelationshipDto>>();
            var relationship = await _service.GetAllRelationship();
            result.Successful = true;
            result.Data = relationship;
            return result;
        }

        [HttpPost]
        [Route("rule/relationship/insert")]
        public async Task<DataResponseDto<bool>> Insert([FromHeader(Name = "LoginUser")] string user, [FromBody] RelationshipDto relationshipDto)
        {
            var result = new DataResponseDto<bool>();

            if (relationshipDto != null)
            {
                //创建信息
                relationshipDto.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                relationshipDto.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Create(relationshipDto);
                result.Successful = rResult;
                result.ErrorMessage = rResult?"":"创建失败,请检查类型及是否重复!";
            }
            else
            {
                result.Successful = false;
                result.ErrorMessage = "传入参数为空！";
            }
            return result;
        }


        [HttpPost]
        [Route("rule/relationship/delete")]
        public async Task<DataResponseDto<bool>> Delete([FromBody] RelationshipDto relationshipDto)
        {
            var result = new DataResponseDto<bool>();

            if (relationshipDto != null)
            {
                var rResult = await _service.Delete(relationshipDto);
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
        [Route("rule/relationship/update")]
        public async Task<DataResponseDto<bool>> Update([FromHeader(Name = "LoginUser")] string user, [FromBody] RelationshipDto relationshipDto)
        {
            var result = new DataResponseDto<bool>();

            if (relationshipDto != null)
            {
                //创建信息
                relationshipDto.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                relationshipDto.CreaterBy = JsonConvert.DeserializeObject<UserDto>(user).DisplayName;
                var rResult = await _service.Update(relationshipDto);
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


        [HttpGet]
        [Route("rule/getTopics")]
        public async Task<DataResponseDto<IEnumerable<string>>> GetTopics()
        {
            var result = new DataResponseDto<IEnumerable<string>>();
            var topics = await _service.GetTopics();
            result.Data = topics;
            result.Successful = true;
            result.ErrorMessage = string.Empty;
            return result;
        }
    }
}
