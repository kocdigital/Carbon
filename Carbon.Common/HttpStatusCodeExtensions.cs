using System.Net;

namespace Carbon.Common
{
    public static class HttpStatusCodeExtensions
    {
        /// <summary>
        /// Converts <see cref="HttpStatusCode"/> to <see cref="ApiStatusCode"/>
        /// </summary>
        /// <param name="httpStatusCode">Status code to convert</param>
        /// <returns>An equivalent <see cref="ApiStatusCode"/> for given <see cref="HttpStatusCode"/></returns>
        public static ApiStatusCode GetApiStatusCode(this HttpStatusCode httpStatusCode) => ApiStatusCodeToHttpStatusMapper.GetApiStatusCode(httpStatusCode);
    }
}
