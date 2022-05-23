using System;
using System.Net.Http;

namespace TaskSchedulerClient.ErrorHandling
{
    public class ServerException : Exception
    {
        public HttpResponseMessage ResponseMessage { get; set; }
        public ServerException(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }
    }
}
