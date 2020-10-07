using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeExtensionsWrapper
{
    public class ApiStatusCodeExtensionsWrapper : IApiStatusCodeExtensionsWrapper
    {
        public HttpStatusCode GetHttpStatusCode(ApiStatusCode apiStatusCode)
        {
          return  ApiStatusCodeExtensions.GetHttpStatusCode(apiStatusCode);
        }
    }
}
