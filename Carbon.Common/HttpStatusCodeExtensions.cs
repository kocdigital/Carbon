using System.Net;

namespace Carbon.Common
{
    public static class HttpStatusCodeExtensions
    {
        public static ApiStatusCode GetApiStatusCode(this HttpStatusCode httpStatusCode)
        {
            return ApiStatusCodeToHttpStatusMapper.GetApiStatusCode(httpStatusCode);
        }
    }
}
