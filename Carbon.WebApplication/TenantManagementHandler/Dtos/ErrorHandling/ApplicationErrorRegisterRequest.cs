using System.Collections.Generic;


namespace Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling
{
    public class ApplicationErrorRegisterRequest
    {
        public ApplicationErrorRegisterRequest(string applicationCode, List<ApplicationErrorTranslation> applicationErrorTranslations)
        {
            ApplicationCode = applicationCode;
            ApplicationName = applicationCode.Replace(".", " ");
            ApplicationErrorTranslations = applicationErrorTranslations;
        }
        public string ApplicationCode { get; set; }
        public string ApplicationName { get; set; }
        public List<ApplicationErrorTranslation> ApplicationErrorTranslations { get; set; }
    }
}
