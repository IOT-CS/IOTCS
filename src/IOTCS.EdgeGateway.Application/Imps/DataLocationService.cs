using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Domain.ValueObject.Notification;
using IOTCS.EdgeGateway.Freesql.Helper;
using IOTCS.EdgeGateway.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class DataLocationService : IDataLocationService
    {
        private readonly ILogger _logger;
        private readonly IDataLocationRepository _repository;
        private ConcurrentDictionary<string, NotifyChangeDto> _keyValues;

        public DataLocationService(ILogger logger
            , ConcurrentDictionary<string, NotifyChangeDto> keyValues
            , IDataLocationRepository repository)
        {
            this._logger = logger;
            this._repository = repository;
            this._keyValues = keyValues;
        }

        public async Task<IEnumerable<DataLocationDto>> GetAsync()
        {
            var list = await _repository.GetAsync();

            var query = from s in list
                        select s.ToModel<DataLocationModel, DataLocationDto>();


            return query;
        }

        public async Task<IEnumerable<DataLocationDto>> GetNodeByIdAsync(string id)
        {
            var result = new List<DataLocationDto>();
            var model = await _repository.GetNodeByIdAsync(id).ConfigureAwait(false);
            var node = from s in model
                       select s.ToModel<DataLocationModel, DataLocationDto>();

            if (_keyValues.ContainsKey(id))
            {
                var notify = _keyValues[id];
                DataLocationDto dataLocation = null;
                foreach (var locaton in notify.Nodes)
                {
                    dataLocation = node.Where(w => w.Id == locaton.Id).FirstOrDefault();
                    if (dataLocation != null)
                    {
                        dataLocation.Source = locaton.Source;
                        dataLocation.Sink = locaton.Sink;
                        dataLocation.Status = locaton.Status;

                        result.Add(dataLocation);
                    }
                }
            }
            else
            {
                return node;
            }
            return result;
        }

        public async Task<bool> Insert(DataLocationDto data)
        {
            var result = true;
            var model = data.ToModel<DataLocationDto, DataLocationModel>();

            var rResult = _repository.Insert(model);

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Update(DataLocationDto data)
        {
            var result = true;
            var model = data.ToModel<DataLocationDto, DataLocationModel>();

            await _repository.Update(model);

            return await Task.FromResult<bool>(result);
        }
        public async Task<bool> Delete(DataLocationDto data)
        {
            var result = true;
            var model = data.ToModel<DataLocationDto, DataLocationModel>();

            await _repository.Delete(model);

            return await Task.FromResult<bool>(result);
        }
    }
}
