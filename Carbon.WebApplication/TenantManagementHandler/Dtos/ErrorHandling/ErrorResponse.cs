using System;

namespace Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {

        }
        public ErrorResponse(string errorDescription, long errorCode, string languageCode)
        {
            ErrorDescription = errorDescription;
            ErrorCode = errorCode;
            LanguageCode = languageCode;
        }
        public string ErrorDescription { get; set; }
        public long ErrorCode { get; set; }
        public string LanguageCode { get; set; }
        public string ApplicationCode { get; set; }
        public Guid? SolutionId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
