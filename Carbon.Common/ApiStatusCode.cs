namespace Carbon.Common
{
    /// <summary>
    /// Indicates operation status
    /// </summary>
    public enum ApiStatusCode
    {
        /// <summary>
        /// Indicates that the operation was successful
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.OK"/></remarks>
        OK = 2000,
        /// <summary>
        /// Indicates that the request is not suitable
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.BadRequest"/></remarks>
        BadRequest = 4000,
        /// <summary>
        /// Indicates that the request is unauthorized
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.Unauthorized"/></remarks>
        UnAuthorized = 4001,
        /// <summary>
        /// Indicates that the requested resource is not exist
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.NotFound"/></remarks>
        NotFound = 4004,
        /// <summary>
        /// indicates that the client did not send a request within the time the server was expecting the request.
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.RequestTimeout"/></remarks>
        RequestTimeout = 4008,
        /// <summary>
        /// indicates that the request could not be carried out because of a conflict on the operation.
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.Conflict"/></remarks>
        Conflict = 4009,
        /// <summary>
        /// indicates that a generic error has occurred while operation.
        /// </summary>
        /// <remarks>Similar to <see cref="System.Net.HttpStatusCode.InternalServerError"/></remarks>
        InternalServerError = 5000
    }
}
