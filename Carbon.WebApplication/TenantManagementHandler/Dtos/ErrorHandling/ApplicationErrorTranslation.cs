namespace Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling
{
    public class ApplicationErrorTranslation
    {
        public long ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        public string LanguageName { get; set; }
        public string LanguageCode { get; set; }
    }
}
