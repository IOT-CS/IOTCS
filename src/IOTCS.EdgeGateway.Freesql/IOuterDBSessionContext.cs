
namespace IOTCS.EdgeGateway.Freesql
{
    public interface IOuterDBSessionContext
    {
        IFreeSql CreateDbContext();
    }
}