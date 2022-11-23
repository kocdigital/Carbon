
namespace Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling
{
    /// <summary>
	/// Request object for to get application based errors for given language and tenant
	/// </summary>
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
