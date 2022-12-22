using System.Net;

namespace Carbon.Common
{

    public static class ApiStatusCodeExtensions
    {
        /// <summary>
        /// Converts <see cref="ApiStatusCode"/> to <see cref="HttpStatusCode"/>
        /// </summary>
        /// <param name="apiStatusCode">Status code to convert</param>
        /// <returns>An equivalent <see cref="HttpStatusCode"/> for given <see cref="ApiStatusCode"/></returns>
        public static HttpStatusCode GetHttpStatusCode(this ApiStatusCode apiStatusCode) => ApiStatusCodeToHttpStatusMapper.GetHttpStatusCode(apiStatusCode);
    }
}
