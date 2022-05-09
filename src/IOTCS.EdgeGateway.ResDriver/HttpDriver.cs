using IOTCS.EdgeGateway.ComResDriver;
using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IOTCS.EdgeGateway.ResDriver
{
    public class HttpDriver : IHttpDriver, IDisposable
    {
        private RestClient _httpClient;

        public dynamic Parameter { get; set; }

        private Method HttpMethod { get; set; }

        private Uri HttpUri { get; set; }

        public void Initialize(string config)
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
            result = response.IsSuccessful;
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
