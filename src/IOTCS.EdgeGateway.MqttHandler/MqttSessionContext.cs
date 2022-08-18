using IOTCS.EdgeGateway.MqttHandler.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Security.Authentication;

namespace IOTCS.EdgeGateway.MqttHandler
{
    public class MqttSessionContext : IMqttSessionContext
    {   
        public async Task<IMqttClient> CreateMqttContextAsync(MqttContextOptions options)
        {
            var token = new CancellationToken();
            var mqttClient = new MqttFactory().CreateMqttClient();
            if (string.IsNullOrEmpty(options.UserId))
            {
                var connector = await mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(options.IpAddress, options.Port)
                .WithClientId(options.ClientId)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false,
                    AllowUntrustedCertificates = true,
                    SslProtocol = SslProtocols.Tls12
                })
                .WithCommunicationTimeout(TimeSpan.FromSeconds(options.Timeout))
                .Build(), token);
            }
            else
            {
                var connector = await mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(options.IpAddress, options.Port)
                .WithClientId(options.ClientId)
                .WithCredentials(options.UserId, options.Password)
                .WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    UseTls = false,
                    AllowUntrustedCertificates = true,
                    SslProtocol = SslProtocols.Tls12
                })
                .WithCommunicationTimeout(TimeSpan.FromSeconds(options.Timeout))
                .Build(), token);
            }

            return mqttClient;
        }
    }    
}
