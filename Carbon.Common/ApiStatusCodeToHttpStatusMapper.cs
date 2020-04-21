using System;
using System.Collections.Generic;
using System.Net;

namespace Carbon.Common
{
    public static class ApiStatusCodeToHttpStatusMapper
    {
        public static IDictionary<ApiStatusCode, HttpStatusCode> ApiStatusCodes = new Dictionary<ApiStatusCode, HttpStatusCode>
        {
            { ApiStatusCode.OK, HttpStatusCode.OK },
            { ApiStatusCode.BadRequest, HttpStatusCode.BadRequest },
            { ApiStatusCode.Conflict, HttpStatusCode.Conflict },
            { ApiStatusCode.InternalServerError, HttpStatusCode.InternalServerError },
            { ApiStatusCode.NotFound, HttpStatusCode.NotFound },
            { ApiStatusCode.RequestTimeout, HttpStatusCode.RequestTimeout },
            { ApiStatusCode.UnAuthorized, HttpStatusCode.Unauthorized }
        };

        public static IDictionary<HttpStatusCode, ApiStatusCode> HttpStatusCodes = new Dictionary<HttpStatusCode, ApiStatusCode>
        {
            { HttpStatusCode.OK, ApiStatusCode.OK },
            { HttpStatusCode.Created, ApiStatusCode.OK },
            { HttpStatusCode.NoContent, ApiStatusCode.OK },

            { HttpStatusCode.BadRequest, ApiStatusCode.BadRequest },
            { HttpStatusCode.UpgradeRequired, ApiStatusCode.BadRequest },
            { HttpStatusCode.UnsupportedMediaType, ApiStatusCode.BadRequest },
            { HttpStatusCode.RequestUriTooLong, ApiStatusCode.BadRequest },
            { HttpStatusCode.RequestEntityTooLarge, ApiStatusCode.BadRequest },
            { HttpStatusCode.RequestedRangeNotSatisfiable, ApiStatusCode.BadRequest },
            { HttpStatusCode.ProxyAuthenticationRequired, ApiStatusCode.BadRequest },
            { HttpStatusCode.PaymentRequired, ApiStatusCode.BadRequest },
            { HttpStatusCode.NotAcceptable, ApiStatusCode.BadRequest },
            { HttpStatusCode.MethodNotAllowed, ApiStatusCode.BadRequest },
            { HttpStatusCode.Gone, ApiStatusCode.BadRequest },
            { HttpStatusCode.ExpectationFailed, ApiStatusCode.BadRequest },
            { HttpStatusCode.LengthRequired, ApiStatusCode.BadRequest },

            { HttpStatusCode.Conflict, ApiStatusCode.Conflict },

            { HttpStatusCode.GatewayTimeout, ApiStatusCode.InternalServerError },
            { HttpStatusCode.ServiceUnavailable, ApiStatusCode.InternalServerError },
            { HttpStatusCode.HttpVersionNotSupported, ApiStatusCode.InternalServerError },
            { HttpStatusCode.NotImplemented, ApiStatusCode.InternalServerError },
            { HttpStatusCode.BadGateway, ApiStatusCode.InternalServerError },
            { HttpStatusCode.InternalServerError, ApiStatusCode.InternalServerError },
            { HttpStatusCode.LoopDetected, ApiStatusCode.InternalServerError },
            { HttpStatusCode.InsufficientStorage, ApiStatusCode.InternalServerError },
            { HttpStatusCode.VariantAlsoNegotiates, ApiStatusCode.InternalServerError },

            { HttpStatusCode.NotFound, ApiStatusCode.NotFound},
            { HttpStatusCode.RequestTimeout, ApiStatusCode.RequestTimeout },
            { HttpStatusCode.Unauthorized, ApiStatusCode.UnAuthorized },
            { HttpStatusCode.Forbidden, ApiStatusCode.UnAuthorized },
            
        };

        public static HttpStatusCode GetHttpStatusCode(ApiStatusCode apiStatusCode)
        {
            if (ApiStatusCodes.TryGetValue(apiStatusCode, out var httpStatusCode))
                return httpStatusCode;

            throw new ArgumentOutOfRangeException($"ApiStatusCode {apiStatusCode} is not mapped with a HttpStatusCode!");
        }

        public static ApiStatusCode GetApiStatusCode(HttpStatusCode httpStatusCode)
        {
            if (HttpStatusCodes.TryGetValue(httpStatusCode, out var apiStatusCode))
                return apiStatusCode;

            throw new ArgumentOutOfRangeException($"HttpStatusCode {httpStatusCode} is not mapped with a ApiStatusCode!");
        }

    }
}
