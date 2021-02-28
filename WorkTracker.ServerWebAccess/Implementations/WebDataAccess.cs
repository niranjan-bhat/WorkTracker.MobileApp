using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.WebAccess.Implementations
{
    public class WebDataAccess : WorkTrackerHttpClient, IWebDataAccess
    {
        private string _baseUri = "";
        public WebDataAccess(IConfiguration config) : base(config)
        {
        }
        public async Task<T> GetAsync<T>(string requestUrl)
        {
            var httpClient = GetHttpClient();

            var response = await httpClient.GetAsync(requestUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }

            throw await RetrieveHttpCallFailure(response);
        }

        public async Task<T> PostAsync<T>(string requestUrl,HttpContent obj)
        {
            var httpClient = GetHttpClient();

            var response = await httpClient.PostAsync(requestUrl,obj);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }

            throw await RetrieveHttpCallFailure(response);
        }

    }
}
