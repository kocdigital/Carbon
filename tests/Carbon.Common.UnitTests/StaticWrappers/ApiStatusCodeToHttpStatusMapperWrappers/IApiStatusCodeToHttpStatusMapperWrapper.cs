using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeToHttpStatusMapperWrappers
{
    public interface IApiStatusCodeToHttpStatusMapperWrapper
    {
        ApiStatusCode GetApiStatusCode(HttpStatusCode httpStatusCode);
        HttpStatusCode GetHttpStatusCode(ApiStatusCode apiStatusCode);
    }
}
