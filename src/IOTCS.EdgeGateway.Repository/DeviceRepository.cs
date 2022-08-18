using IOTCS.EdgeGateway.Domain.Models;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;
using IOTCS.EdgeGateway.Freesql.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IFreeSql _freeSql;

        public DeviceRepository(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }


        public async Task<bool> Create(DeviceModel deviceModel)
        {
            var currRow = new List<DeviceModel>();
            if (deviceModel.DeviceType == "0")
            {
                currRow = await _freeSql.Select<DeviceModel>()
                .Where(d => d.DeviceType.Equals("0") && d.DeviceName.Equals(deviceModel.DeviceName))
                .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                currRow = await _freeSql.Select<DeviceModel>()
                .Where(d => d.ParentId.Equals(deviceModel.ParentId) && (d.DeviceName.Equals(deviceModel.DeviceName) || d.Topic.Equals(deviceModel.Topic)))
                .ToListAsync().ConfigureAwait(false);
            }

            if (currRow != null && currRow.Count > 0)
            {
                return false;
            }
            var affrows = _freeSql.Insert(deviceModel).ExecuteAffrows();
            bool result = affrows > 0;
            return await Task.FromResult<bool>(result);
        }



        public async Task<bool> Update(DeviceModel deviceModel)
        {
            var currRow = new List<DeviceModel>();
            if (deviceModel.DeviceType == "0")
            {
                currRow = await _freeSql.Select<DeviceModel>()
                .Where(d => d.DeviceName.Equals(deviceModel.DeviceName))
                .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                currRow = await _freeSql.Select<DeviceModel>()
                .Where(d => d.ParentId.Equals(deviceModel.ParentId) && (d.DeviceName.Equals(deviceModel.DeviceName) || d.Topic.Equals(deviceModel.Topic)))
                .ToListAsync().ConfigureAwait(false);
            }

            if (currRow != null && currRow.Count > 1)
            {
                return false;
            }
            var result = false;

            var affrows = _freeSql.Update<DeviceModel>().SetSource(deviceModel).ExecuteAffrows();

            result = affrows > 0 ? true : false;

            return await Task.FromResult<bool>(result);
        }

        public async Task<bool> Delete(DeviceModel deviceModel)
        {
            var result = false;
            StringBuilder stringBuilder = new StringBuilder();

            var deviceGroup = await _freeSql.Select<DeviceModel>().Where(d => d.Id == deviceModel.Id).ToListAsync().ConfigureAwait(false);
            if (deviceGroup != null && deviceGroup.Count > 0)
            {
                //删除设备组的同时删除设备以及设备配置
                if (deviceGroup[0].DeviceType == "0")
                {
                    var devices = await _freeSql.Select<DeviceModel>().Where(d => d.ParentId == deviceModel.Id).ToListAsync().ConfigureAwait(false);
                    if (devices != null)
                    {
                        foreach (var device in devices)
                        {
                            var affrow = _freeSql.Delete<DeviceModel>().Where(d => d.Id == device.Id).ExecuteAffrows();
                            if (affrow > 0)
                            {
                                _freeSql.Delete<DeviceConfigModel>().Where(d => d.DeviceId == deviceModel.Id).ExecuteAffrows();
                                _freeSql.Delete<DataLocationModel>().Where(d => d.ParentId == deviceModel.Id).ExecuteAffrows();
                            }
                        }
                        var affrows = _freeSql.Delete<DeviceModel>().Where(d => d.Id == deviceModel.Id).ExecuteAffrows();
                        result = affrows > 0 ? true : false;
                    }
                }
                else
                {
                    var affrows = _freeSql.Delete<DeviceModel>().Where(d => d.Id == deviceModel.Id).ExecuteAffrows();
                    if (affrows > 0)
                    {
                        _freeSql.Delete<DeviceConfigModel>().Where(d => d.DeviceId == deviceModel.Id).ExecuteAffrows();
                        _freeSql.Delete<DataLocationModel>().Where(d => d.ParentId == deviceModel.Id).ExecuteAffrows();
                    }

                    result = affrows > 0 ? true : false;

                }
            }
            return await Task.FromResult<bool>(result);
        }

        public async Task<IEnumerable<DeviceModel>> GetAsync()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<DeviceModel> result = new List<DeviceModel>();

            var query = await _freeSql.Select<DeviceModel>().ToListAsync().ConfigureAwait(false);
            if (query != null)
            {
                result.AddRange(query);
            }

            return result;
        }

        public async Task<IEnumerable<DeviceModel>> GetGroupByDeviceId(string deviceId)
        {
            List<DeviceModel> result = new List<DeviceModel>();
            //设备组
            var deviceGroup = await _freeSql.Select<DeviceModel>().Where(d => d.ParentId == deviceId).ToListAsync().ConfigureAwait(false);
            if (deviceGroup != null)
                result.AddRange(deviceGroup);
            return result;
        }

        public async Task<IEnumerable<DeviceModel>> GetDeviceGroup()
        {
            List<DeviceModel> result = new List<DeviceModel>();
            var affrows = await _freeSql.Select<DeviceModel>().Where(d => d.DeviceType == "0").ToListAsync().ConfigureAwait(false);
            if (affrows != null)
                result.AddRange(affrows);
            return result;
        }

        public async Task<string> GetNodeTypeConfigByDriveType(string driveType)
        {
            var result = await _freeSql.Select<NodeTypeConfigModel>().Where(d => d.DriveType.Equals(driveType)).ToListAsync().ConfigureAwait(false);
            if (result != null && result.Count > 0)
                return result.FirstOrDefault().NodeTypeJson;
            return string.Empty;
        }
    }
}
