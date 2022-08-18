using IOTCS.EdgeGateway.ComResDriver;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.ResDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class ResourceService : IResourceService
    {
        private readonly ILogger _logger;
        private readonly IResourceRepository _repository;
        private readonly IMqttDriver _mqttDriver;
        private readonly IHttpDriver _httpDriver;
        public ResourceService(IServiceProvider services
            , ILogger logger
            , IResourceRepository repository)
        {
            this._mqttDriver = new MqttDriver();
            this._httpDriver = new HttpDriver();
            this._logger = logger;
            this._repository = repository;
        }

        public async Task<IEnumerable<ResourceDto>> GetAsync()
        {
            var list = await _repository.GetAsync();

            var query = from s in list
                        select s.ToModel<ResourceModel, ResourceDto>();

            return query;
        }

        public async Task<bool> Insert(ResourceDto data)
        {
            var result = true;
            var model = data.ToModel<ResourceDto, ResourceModel>();

            result = await _repository.Insert(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Update(ResourceDto data)
        {
            var result = true;
            var model = data.ToModel<ResourceDto, ResourceModel>();

            result = await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(ResourceDto data)
        {
            var result = true;
            var model = data.ToModel<ResourceDto, ResourceModel>();

            result = await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> TestAsync(ResourceDto data)
        {
            var result = true;
            var rResult = string.Empty;
            var model = data.ToModel<ResourceDto, ResourceModel>();

            switch (model.ResourceType.ToLower())
            {
                case "mqtt":
                    rResult = _mqttDriver.CheckConnected(model.ResourceParams);
                    if (string.IsNullOrEmpty(rResult))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case "webhook":
                    rResult = _httpDriver.CheckConnected(model.ResourceParams);
                    if (string.IsNullOrEmpty(rResult))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
            }

            return await Task.FromResult<bool>(result);
        }
    }
}
