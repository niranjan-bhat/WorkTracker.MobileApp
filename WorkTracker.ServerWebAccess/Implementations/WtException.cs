using System;
using System.Runtime.Serialization;

namespace WorkTracker.WebAccess.Implementations
{
    public class WtException : Exception
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}