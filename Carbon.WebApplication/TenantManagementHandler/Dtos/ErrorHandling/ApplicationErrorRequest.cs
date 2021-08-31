
namespace Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling
{
    public class ApplicationErrorRequest
    {
        public ApplicationErrorRequest(
            string applicationCode,
            long errorCode,
            string languageCode,
            string tenantId,
            string solutionId)
        {
            ApplicationCode = applicationCode;
            ErrorCode = errorCode;
            LanguageCode = languageCode;
            TenantId = tenantId;
            SolutionId = solutionId;
        }
        public string ApplicationCode { get; set; }
        public long ErrorCode { get; set; }
        public string LanguageCode { get; set; }

        public string TenantId { get; set; }
        public string SolutionId { get; set; }
    }
}
