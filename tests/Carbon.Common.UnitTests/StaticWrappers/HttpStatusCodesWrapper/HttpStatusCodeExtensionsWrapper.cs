using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.HttpStatusCodeExtensionsWrapper
{
    public class HttpStatusCodeExtensionsWrapper : IHttpStatusCodeExtensionsWrapper
    {
        public ApiStatusCode GetApiStatusCode(HttpStatusCode httpStatusCode)
        {
            return HttpStatusCodeExtensions.GetApiStatusCode(httpStatusCode);
        }
    }
}
