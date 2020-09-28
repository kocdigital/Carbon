using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeExtensionsWrapper
{
    public interface IApiStatusCodeExtensionsWrapper
    {
      HttpStatusCode GetHttpStatusCode(ApiStatusCode apiStatusCode);
       
    }
}
