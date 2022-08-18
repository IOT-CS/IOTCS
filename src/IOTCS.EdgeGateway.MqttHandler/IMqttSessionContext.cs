using IOTCS.EdgeGateway.MqttHandler.Extensions;
using MQTTnet.Client;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.MqttHandler
{
    public interface IMqttSessionContext
    {
        Task<IMqttClient> CreateMqttContextAsync(MqttContextOptions options);
    }
}
