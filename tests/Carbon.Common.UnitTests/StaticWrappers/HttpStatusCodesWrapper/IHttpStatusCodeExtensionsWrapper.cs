using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.HttpStatusCodeExtensionsWrapper
{
    public interface IHttpStatusCodeExtensionsWrapper
    {
        ApiStatusCode GetApiStatusCode(HttpStatusCode httpStatusCode);
    }
}
