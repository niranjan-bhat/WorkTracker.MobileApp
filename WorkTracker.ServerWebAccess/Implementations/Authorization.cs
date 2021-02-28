using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.WebAccess.Implementations
{
    public class Authorization : WorkTrackerHttpClient, IAuthorization
    {
        public Authorization(IConfiguration config) : base(config)
        {
        }

        public async Task<bool> AuthorizeUser(string email, string password)
        {
            var httpClient = GetHttpClient();
            var emailUrlEncoded = HttpUtility.UrlEncode(email);
            var passwordUrlEncoded = HttpUtility.UrlEncode(password);
            var url = $"Owner/Authenticate?email={emailUrlEncoded}&password={passwordUrlEncoded}";

            var response = await httpClient.PostAsync(url, null);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _webConfig.AuthToken = await response.Content.ReadAsStringAsync();
                return true;
            }

            throw await RetrieveHttpCallFailure(response);
        }
    }
}
