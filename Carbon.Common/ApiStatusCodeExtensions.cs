using System.Net;

namespace Carbon.Common
{

    public static class ApiStatusCodeExtensions
    {
        public static HttpStatusCode GetHttpStatusCode(this ApiStatusCode apiStatusCode)
        {
            return ApiStatusCodeToHttpStatusMapper.GetHttpStatusCode(apiStatusCode);
        }
    }
}
