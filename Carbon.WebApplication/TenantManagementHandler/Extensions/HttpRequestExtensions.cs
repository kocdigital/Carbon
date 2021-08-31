using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Carbon.WebApplication.TenantManagementHandler.Extensions
{
    public static class HttpRequestExtensions
    {
        public const string DefaultLanguage = "tr-TR";
        public static string GetUserLanguage(this HttpRequest request)
        {
            var languages = request.GetTypedHeaders()
                .AcceptLanguage
                ?.OrderByDescending(x => x.Quality ?? 1)
                .Select(x => x.Value.ToString())
                .ToArray() ?? Array.Empty<string>();


            if (languages != null && languages.Length > 0)
            {
                return languages[0];
            }

            return DefaultLanguage;
        }
    }
}
