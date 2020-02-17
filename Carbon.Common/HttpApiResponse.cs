using System.Collections.Generic;
using System.Net;

namespace Carbon.Common
{
    public class HttpApiResponse<T> : ApiResponse<T>
    {

        public HttpApiResponse(string identifier, HttpStatusCode statusCode) : base(identifier, statusCode.GetApiStatusCode())
        {
           
        }

        public HttpApiResponse(string identifier, HttpStatusCode statusCode, int errorCode) : base(identifier, statusCode.GetApiStatusCode(), errorCode)
        {
        }

        public HttpApiResponse(string identifier, HttpStatusCode statusCode, IList<string> messages) : base(identifier, statusCode.GetApiStatusCode(), messages)
        {
        }

        public HttpApiResponse(string identifier, HttpStatusCode statusCode, int errorCode, IList<string> messages) : base(identifier, statusCode.GetApiStatusCode(), errorCode, messages)
        {
        }
    }


}
