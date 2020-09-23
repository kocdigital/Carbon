using System.Net;

namespace Carbon.Common.UnitTests.StaticWrappers.ApiStatusCodeToHttpStatusMapperWrappers
{
    public class ApiStatusCodeToHttpStatusMapperWrapper : IApiStatusCodeToHttpStatusMapperWrapper
    {
        public ApiStatusCode GetApiStatusCode(HttpStatusCode httpStatusCode)
        {
            return ApiStatusCodeToHttpStatusMapper.GetApiStatusCode(httpStatusCode);
        }
    }
}
