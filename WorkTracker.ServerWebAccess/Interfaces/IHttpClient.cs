using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WorkTracker.WebAccess.Interfaces
{
    public interface IHttpClient
    {
        HttpClient GetHttpClient(); 
    }
}
