using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;

namespace IOTCS.EdgeGateway.Repository
{
    public class DeviceConfigRepository : IDeviceConfigRepository
    {
        private readonly IFreeSql _freeSql;


        public DeviceConfigRepository(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        public async Task<bool> Create(DeviceConfigModel configModel)
        {
            var affrows = _freeSql.Insert(configModel).ExecuteAffrows();
            bool result = affrows > 0;
            return await Task.FromResult<bool>(result);
        }

        public async Task<DeviceConfigModel> GetAllDeviceConfigByDeviceId(string deviceId)
        {
            DeviceConfigModel result = new DeviceConfigModel();
            var config = await _freeSql.Select<DeviceConfigModel>().Where(d => d.DeviceId == deviceId).ToListAsync().ConfigureAwait(false);

            if (config != null && config.Count > 0)
            {
                result = config[0];
            }
            return result;
        }

        public async Task<bool> Update(DeviceConfigModel configDto)
        {
            var result = false;

            var affrows = _freeSql.Update<DeviceConfigModel>().SetSource(configDto).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<DeviceConfigModel>> GetAsync()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<DeviceConfigModel> result = new List<DeviceConfigModel>();

            stringBuilder.Clear();
            stringBuilder.Append("SELECT * FROM tb_deviceconfig");

            var query = await _freeSql.Select<DeviceConfigModel>().WithSql(stringBuilder.ToString()).ToListAsync().ConfigureAwait(false);
            if (query != null)
            {
                result.AddRange(query);
            }

            return result;
        }
    }
}
