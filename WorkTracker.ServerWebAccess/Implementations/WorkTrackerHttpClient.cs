using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.WebAccess.Implementations
{
    public class WorkTrackerHttpClient : IHttpClient
    {
        protected IConfiguration _webConfig;
        public WorkTrackerHttpClient(IConfiguration config)
        {
            _webConfig = config;
        }
        public HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_webConfig.BaseUrl);
            AddAuthorizationHeader(httpClient);
            return httpClient;
        }

        void AddAuthorizationHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _webConfig.AuthToken);
        }

        public async Task<Exception> RetrieveHttpCallFailure(HttpResponseMessage response)
        {
            var message = await response.Content.ReadAsStringAsync();
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    var ex = JsonConvert.DeserializeObject<WtException>(message);
                    return ex ?? new Exception(message);
                default:
                    return new Exception(message);
            }
        }
    }
}
