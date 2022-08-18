using Microsoft.Extensions.Options;

namespace IOTCS.EdgeGateway.Freesql.Extensions
{
    public class OuterDbContextOptions : IOptions<OuterDbContextOptions>
    {
        public string DbType { get; set; } = "1";

        public string DbIp { get; set; }

        public string DbPort { get; set; }

        public string Database { get; set; }

        public string DbUid { get; set; }

        public string DbPwd { get; set; }

        public OuterDbContextOptions Value => this;
    }
}
