using IOTCS.EdgeGateway.ComResDriver;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IOTCS.EdgeGateway.Logging;
using IOTCS.EdgeGateway.Core;

namespace IOTCS.EdgeGateway.ResDriver
{
    public class HttpDriver : IHttpDriver, IDisposable
    {
        private RestClient _httpClient;
        private readonly ILogger _logger;

        public HttpDriver()
        {
            _logger = IocManager.Instance.GetService<ILoggerFactory>().CreateLogger("Monitor");
        }

        public dynamic Parameter { get; set; }

        private Method HttpMethod { get; set; }

        private Uri HttpUri { get; set; }

        public string Initialize(string config)
        {
            var result = string.Empty;

            try
            {
                this.Parameter = JsonConvert.DeserializeObject<dynamic>(config);
                this.HttpUri = new Uri(Convert.ToString(Parameter.HTTPUrl));
                var uriString = HttpUri.Scheme + "://" + HttpUri.Host + ":" + HttpUri.Port;
                var connectionTimeout = Convert.ToInt32(Parameter.ConnectTimeOut);
                _httpClient = new RestClient(new RestClientOptions { BaseUrl = new Uri(uriString), Timeout = connectionTimeout });
                this.HttpMethod = Method.Post;
                String stringMethod = Parameter.HTTPMethod;
                switch (stringMethod.ToLower())
                {
                    case "get":
                        this.HttpMethod = Method.Get;
                        break;
                    case "post":
                        this.HttpMethod = Method.Post;
                        break;
                }                
            }
            catch (Exception e)
            {
                var msg = $"Http 初始化错误信息 => {e.Message},错误位置 => {e.StackTrace}";
                result = msg;
                _logger.Info(msg);
            }

            return result;
        }

        public string CheckConnected(string config)
        {
            var result = string.Empty;

            try
            {
                var parameter = JsonConvert.DeserializeObject<dynamic>(config);
                var httpUri = new Uri(Convert.ToString(Parameter.HTTPUrl));
                var uriString = httpUri.Scheme + "://" + httpUri.Host + ":" + httpUri.Port;
                var connectionTimeout = Convert.ToInt32(parameter.ConnectTimeOut);
                var httpClient = new RestClient(new RestClientOptions { BaseUrl = new Uri(uriString), Timeout = connectionTimeout });
            }
            catch (Exception e)
            {
                var msg = $"Http 初始化错误信息 => {e.Message},错误位置 => {e.StackTrace}";
                result = msg;
                _logger.Info(msg);
            }

            return result;
        }

        public async Task<bool> Run(dynamic data)
        {
            var result = false;

            var restRequest = new RestRequest(HttpUri.AbsolutePath, this.HttpMethod)
            {
                Timeout = Convert.ToInt32(Parameter.ReqTimeOut),
            };
            String headerStringArray = Parameter.Headers == null ? string.Empty : Convert.ToString(Parameter.Headers);
            if (!string.IsNullOrEmpty(headerStringArray))
            {
                var headers = JsonConvert.DeserializeObject<IEnumerable<CustomHeader>>(headerStringArray);
                foreach (var h in headers)
                {
                    restRequest.AddHeader(h.key, h.value);
                }
            }
            var req = new RequestDto { ResourceId = "iotcs_test", Msg = JsonConvert.SerializeObject(data) };
            restRequest.AddJsonBody<RequestDto>(req);
            var response = await _httpClient.ExecuteAsync(restRequest).ConfigureAwait(false);
            result = response.get_IsSuccessful();
            restRequest = null;

            return result;
        }

        public bool IsConnected()
        {
            return true;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class CustomHeader
    {
        public string key { get; set; }

        public string value { get; set; }
    }

    public class RequestDto
    {
        public string ResourceId { get; set; }

        public string Msg { get; set; }
    }
}
