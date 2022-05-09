namespace IOTCS.EdgeGateway.Infrastructure.WebApi
{
    public class Settings
    {
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int LifeTime { get; set; }
        public bool ValidateLifetime { get; set; }
    }
}
