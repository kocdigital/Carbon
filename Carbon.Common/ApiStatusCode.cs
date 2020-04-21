namespace Carbon.Common
{
    public enum ApiStatusCode
    {
        OK = 2000,
        BadRequest = 4000,
        UnAuthorized = 4001,
        NotFound = 4004,        
        RequestTimeout = 4008,
        Conflict = 4009,
        InternalServerError = 5000
    }
}
