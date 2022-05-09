

namespace IOTCS.EdgeGateway.Freesql
{
    public interface ICommonDbSessionContext
    {
        IFreeSql CreateDbContext(dynamic options, string type);
    }
}
