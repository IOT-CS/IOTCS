using FreeSql;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace IOTCS.EdgeGateway.Freesql
{
    /// <summary>
    /// 数据库连接的Session对象
    /// </summary>
    public class DBSessionContext : IDBSessionContext
    {
        private IFreeSql _fsql;
        private readonly Extensions.DbContextOptions _options;

        public DBSessionContext(IOptions<Extensions.DbContextOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
        }

        public IFreeSql CreateDbContext()
        {
            var connectionStrings = string.Empty;
            var dbType = GetOptions(_options, out connectionStrings);
            _fsql = new FreeSqlBuilder()
                        .UseConnectionString(dbType, connectionStrings)
                        .UseAutoSyncStructure(true) //自动同步实体结构到数据库
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

        private DataType GetOptions(Extensions.DbContextOptions options, out string conn)
        {
            var connectionStrings = string.Empty;
            DataType dbType = DataType.Oracle;

            switch (options.DbType)
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
                case "4"://PostgreSQL
                    dbType = DataType.PostgreSQL;
                    connectionStrings = $"Server={options.DbIp};Port={options.DbPort};Database={options.Database};User Id={options.DbUid};Password={options.DbPwd};";
                    break;
                case "5"://SQLite
                    dbType = DataType.Sqlite;
                    string dbPath = string.Empty;
                    if (options.Debug)
                        dbPath = Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "Database", "iotcs.db");
                    else
                        dbPath = Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName, "Database", "iotcs.db");
                    connectionStrings = $"Data Source={dbPath}";
                    break;
                default:
                    break;
            }

            conn = connectionStrings;

            return dbType;
        }
    }
}
