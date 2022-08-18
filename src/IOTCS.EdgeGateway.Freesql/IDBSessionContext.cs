
namespace IOTCS.EdgeGateway.Freesql
{
    /// <summary>
    /// 数据连接的Session接口
    /// </summary>
    public interface IDBSessionContext
    {
        IFreeSql CreateDbContext();
    }
}
