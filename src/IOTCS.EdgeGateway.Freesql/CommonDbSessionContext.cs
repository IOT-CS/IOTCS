
using FreeSql;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Dynamic;

namespace IOTCS.EdgeGateway.Freesql
{
    public class CommonDbSessionContext :  ICommonDbSessionContext
    {
        private IFreeSql _fsql;

        public IFreeSql CreateDbContext(dynamic options, string type)
        {
            var connectionStrings = string.Empty;
            var dbType = GetOptions(options, type, out connectionStrings);
            _fsql = new FreeSqlBuilder()
                        .UseConnectionString(dbType, connectionStrings)
                        .UseAutoSyncStructure(false) //自动同步实体结构到数据库
                        .Build(); //请务必定义成 Singleton 单例模式
            if (_fsql == null)
            {
                throw new Exception("数据库类型为空！请检查！");
            }
            else
            {
                return _fsql;
            }
        }

        private DataType GetOptions(dynamic optionsObj, string type, out string conn)
        {
            var connectionStrings = string.Empty;
            DataType dbType = DataType.Oracle;
            dynamic options = JsonConvert.DeserializeObject<ExpandoObject>(optionsObj.ResourceParams);

            switch (type)
            {
                case "1"://Oracle
                    dbType = DataType.Oracle;
                    connectionStrings = $"Data Source={options.DbIp}/{options.Database};Persist Security Info=True;User ID={options.DbUid};Password={options.DbPwd};";
                    break;
                case "2"://SQLServer
                    dbType = DataType.SqlServer;
                    connectionStrings = $"Server={options.DbIp};Database={options.Database};User Id={options.DbUid};Password={options.DbPwd};";
                    break;
                case "3"://Mysql
                    dbType = DataType.MySql;
                    connectionStrings = $"Server={options.DbIp};Database={options.Database};Uid={options.DbUid};Pwd={options.DbPwd};";
                    break;
                case "MySQL"://Mysql
                    dbType = DataType.MySql;
                    connectionStrings = $"Server={options.DbIp};Database={options.Database};Uid={options.DbUid};Pwd={options.DbPwd};";
                    break;
                case "PostgreSQL"://PostgreSQL
                    dbType = DataType.PostgreSQL;
                    connectionStrings = $"Server={options.DbIp};Port={options.DbPort};Database={options.Database};User Id={options.DbUid};Password={options.DbPwd};";
                    break;
                case "4"://PostgreSQL
                    dbType = DataType.PostgreSQL;
                    connectionStrings = $"Server={options.DbIp};Port={options.DbPort};Database={options.Database};User Id={options.DbUid};Password={options.DbPwd};";
                    break;
                case "TDengine"://TDengine
                    dbType = DataType.Sqlite;
                    break;               
                case "5"://Sqlite
                    string iotcs_Path = AppDomain.CurrentDomain.BaseDirectory + "iotcs.db";
                    connectionStrings = $"Data Source={iotcs_Path}";
                    break;
                default:
                    break;
            }

            conn = connectionStrings;

            return dbType;
        }
    }
}
