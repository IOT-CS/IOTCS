using IOTCS.EdgeGateway.Core;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.ComResDriver;
using IOTCS.EdgeGateway.MqttHandler;
using IOTCS.EdgeGateway.MqttHandler.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Diagnostics;

namespace IOTCS.EdgeGateway.ResDriver
{
    public class MqttDriver : IMqttDriver, IDisposable
    {
        private IMqttClient _mqttClient;
        private readonly ILogger _logger;
        private MqttContextOptions _contextOptions;
        private IMqttSessionContext _mqttSessionContext;
        private readonly ISystemDiagnostics _diagnostics;

        public MqttDriver()
        {
            _diagnostics = IocManager.Instance.GetService<ISystemDiagnostics>();
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
            _mqttSessionContext = IocManager.Instance.GetService<IMqttSessionContext>();
        }

        public dynamic Parameter { get; set; }

        public void Initialize(string configData)
        {
            var config = JsonConvert.DeserializeObject<dynamic>(configData);
            _contextOptions = new MqttContextOptions() 
            {
                ClientId = "mqtt" + Guid.NewGuid().ToString("N"),
                IpAddress = config.MQTTIp,
                Port = Convert.ToInt32(config.MQTTPort),
                UserId = config.MQTTUid,
                Password = config.MQTTPwd,
                Timeout = Convert.ToInt32(config.MQTTTimeout),
                IsUsingUser = 0
            };
            Parameter = config;
            var awaiter = _mqttSessionContext.CreateMqttContextAsync(_contextOptions).ConfigureAwait(false).GetAwaiter();
            _mqttClient = awaiter.GetResult();
            _mqttClient.UseDisconnectedHandler(async e =>
            {
                _logger.Info("Mqtt从服务器断开了");
                await Task.Delay(TimeSpan.FromSeconds(3));
                try
                {
                    // 自从 3.0.5 with CancellationToken
                    _mqttClient = await _mqttSessionContext.CreateMqttContextAsync(_contextOptions).ConfigureAwait(false);                    
                    if (_mqttClient.IsConnected)
                    {
                        _logger.Info($"MqttDriver 已经重新连接到服务器");
                    }
                    else
                    {
                        _logger.Info($"MqttDriver 未重新连接到服务器");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info($"Mqtt 重连失败,错误信息 => {ex.Message},错误位置 => {ex.StackTrace}");
                }
            });
        }

        public bool IsConnected()
        {
            var result = false;

            result = _mqttClient == null ? false : _mqttClient.IsConnected;

            return result;
        }

        public async Task<bool> Run(dynamic data)
        {
            var result = false;

            try
            {
                if (_mqttClient.IsConnected)
                {
                    var dataString = JsonConvert.SerializeObject(data);
                    var response = await _mqttClient.PublishAsync(new MqttApplicationMessage() 
                    { 
                        Topic = data.Topic, 
                        Payload = Encoding.UTF8.GetBytes(dataString) 
                    });
                    result = true;
                }
            }
            catch (Exception e)
            {
                result = false;
                var msg = $"MqttDriver 数据发送异常！位置 => {e.Message},异常 => {e.StackTrace}";
                _logger.Info(msg);
                await _diagnostics.PublishDiagnosticsInfoAsync(msg).ConfigureAwait(false);
            }

            return result;
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
