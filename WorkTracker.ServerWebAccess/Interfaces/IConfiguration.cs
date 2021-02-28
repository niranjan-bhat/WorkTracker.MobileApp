using System;
using System.Collections.Generic;
using System.Text;

namespace WorkTracker.WebAccess.Interfaces
{
    public interface IConfiguration
    {
        string BaseUrl { get; }
        string AuthToken { get; set; }
    }
}
