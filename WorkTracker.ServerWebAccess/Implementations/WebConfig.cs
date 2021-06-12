using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.WebAccess.Implementations
{
    public class WebConfig : IConfiguration
    {
        public string BaseUrl { get; } = "http://172.28.48.1:90/worktracker/api/";
        public string AuthToken { get; set; }
    }
}
