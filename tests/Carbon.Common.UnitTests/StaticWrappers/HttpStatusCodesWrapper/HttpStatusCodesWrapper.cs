using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.HttpStatusCodesWrapper
{
    public class HttpStatusCodesWrapper : IHttpStatusCodesWrapper<HttpStatusCode, ApiStatusCode>
    {
        public bool TryGetValue(HttpStatusCode key, out ApiStatusCode value)
        {
            return ApiStatusCodeToHttpStatusMapper.HttpStatusCodes.TryGetValue(key, out value);
        }
    }
}
