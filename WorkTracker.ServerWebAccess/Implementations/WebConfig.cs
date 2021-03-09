using System;
using System.Collections.Generic;
using System.Text;
using WorkTracker.WebAccess.Interfaces;

namespace WorkTracker.WebAccess.Implementations
{
    public class WebConfig : IConfiguration
    {
        public string BaseUrl { get; } = "https://csharpguy.xyz/api/";
        public string AuthToken { get; set; }
    }
}
