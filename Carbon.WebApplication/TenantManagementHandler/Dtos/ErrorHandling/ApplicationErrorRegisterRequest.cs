using System.Collections.Generic;


namespace Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling
{
    /// <summary>
	/// Request object for creating application based translated error definitions
	/// </summary>
    public class ApplicationErrorRegisterRequest
    {
        /// <remarks>
		/// Builds the <see cref="ApplicationName"/> by replacing "." with " " in the <paramref name="applicationCode"/> parameter
		/// </remarks>
        public ApplicationErrorRegisterRequest(string applicationCode, List<ApplicationErrorTranslation> applicationErrorTranslations)
        {
            ApplicationCode = applicationCode;
            ApplicationName = applicationCode.Replace(".", " ");
            ApplicationErrorTranslations = applicationErrorTranslations;
        }
        /// <summary>
		/// Application or API code
		/// </summary>
        public string ApplicationCode { get; set; }
        /// <summary>
		/// Name of the application
		/// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
		/// Error definitions. Can contain multiple languages. For detail see: <see cref="ApplicationErrorTranslation"/>
		/// </summary>
        public List<ApplicationErrorTranslation> ApplicationErrorTranslations { get; set; }
    }
}
