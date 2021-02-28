using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.WebAccess.Interfaces
{
    public interface IWebDataAccess
    {
       Task<T> GetAsync<T>(string requestUrl);
       Task<T> PostAsync<T>(string requestUrl, HttpContent content);
    }
}
