using System;
using System.Runtime.Serialization;

namespace WorkTracker.WebAccess.Implementations
{
    [Serializable]
    public class WebApiException : Exception
    {
        public WebApiException()
        {
        }

        public WebApiException(string message) : base(message)
        {
        }

        public WebApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}