
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Domain.Repositories;
using IOTCS.EdgeGateway.Domain.ValueObject;

namespace IOTCS.EdgeGateway.Application.Imps
{
    public class DBDataStorageService : IDBDataStorageService
    {
        private readonly ILogger _logger;       
        private readonly IOpcStorageRepository _mqttRepository;

        public DBDataStorageService(IOpcStorageRepository repository,
            ILogger logger)            
        {
            this._logger = logger;
            _mqttRepository = repository;
        }

        public async Task<bool> Insert(DataRequestDto request)
        {
            var result = false;
            Stopwatch watch = new Stopwatch();

            watch.Start();            
            if (!string.IsNullOrEmpty(request.SqlTemplate))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(request.InputMessage.ToString());
                var sqlTemplate = string.Empty;

                foreach (var d in dict)
                {
                    sqlTemplate = sqlTemplate.Replace("@{" + d.Key + "}", Convert.ToString(d.Value));
                }
                _logger.Info($"生成SQL语句,=>{sqlTemplate}");
                result = await _mqttRepository.Insert(request.ResourceId, sqlTemplate);
            }
            else
            {
                _logger.Info($"没有找到相应的配置资源或输入数据为空,=>{JsonConvert.SerializeObject(request)}");
            }
            watch.Stop();
            long useTime = watch.ElapsedMilliseconds;
            _logger.Info($"用时=>{(useTime / 1000.0)}秒");

            return result;
        }

        public async Task<bool> BatchInsert(DataRequestDto request)
        {
            var result = false;
            var sqlList = new List<string>();
            Stopwatch watch = new Stopwatch();

            watch.Start();
            if (!string.IsNullOrEmpty(request.InputMessage))
            {
                var list = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(request.InputMessage);
                var sqlTemplate = request.SqlTemplate;
                sqlList.Clear();
                foreach (var dict in list)
                {
                    sqlTemplate = sqlTemplate.Replace("@{" + dict.Key + "}", Convert.ToString(dict.Value), true, null);                    
                }
                sqlList.Add(sqlTemplate);
                _logger.Info($"生成SQL语句,=>{JsonConvert.SerializeObject(sqlList)}");
                result = await _mqttRepository.BatchInsert(request.ResourceId, sqlList);
            }
            else
            {
                _logger.Info($"没有找到相应的配置资源或输入数据为空,=>{JsonConvert.SerializeObject(request)}");
            }
            watch.Stop();
            long useTime = watch.ElapsedMilliseconds;
            _logger.Info($"用时=>{(useTime / 1000.0)}秒");

            return result;
        }

        public async Task<bool> BatchInsert_1(DataRequestDto request)
        {
            var result = false;
            var sqlList = new List<string>();
            Stopwatch watch = new Stopwatch();

            watch.Start();            
            if (!string.IsNullOrEmpty(request.InputMessage))
            {
                var list = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, dynamic>>>(request.InputMessage);
                var sqlTemplate = request.SqlTemplate;
                sqlList.Clear();
                foreach (var dict in list)
                {                   
                    foreach (var d in dict)
                    {
                        sqlTemplate = sqlTemplate.Replace("@{" + d.Key + "}", Convert.ToString(d.Value), true, null);
                    }
                    sqlList.Add(sqlTemplate);
                }
                _logger.Info($"生成SQL语句,=>{JsonConvert.SerializeObject(sqlList)}");
                result = await _mqttRepository.BatchInsert(request.ResourceId, sqlList);
            }
            else
            {
                _logger.Info($"没有找到相应的配置资源或输入数据为空,=>{JsonConvert.SerializeObject(request)}");
            }
            watch.Stop();
            long useTime = watch.ElapsedMilliseconds;
            _logger.Info($"用时=>{(useTime / 1000.0)}秒");

            return result;
        }
    }
}
