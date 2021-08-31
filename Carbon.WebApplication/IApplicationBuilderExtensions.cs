using Carbon.WebApplication.TenantManagementHandler.Services;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Carbon.WebApplication
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder RegisterApplicationError(this IApplicationBuilder app, IWebHostEnvironment env, ErrorHandlingConfig errorHandlingConfig)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetRequiredService<IExternalService>();
                var applicationRegistrationReq = new ApplicationErrorRegisterRequest(
                    env.ApplicationName,
                    GetApplicationErrorRegisterRequest(errorHandlingConfig));

                var result = service.RegisterApplicationError(applicationRegistrationReq).Result;
                Console.WriteLine($"Application Errors Registration Status ==> [{result}]");
            }

            return app;
        }


        private static List<ApplicationErrorTranslation> GetApplicationErrorRegisterRequest(ErrorHandlingConfig errorHandlingConfig)
        {
            var resourceBaseName = errorHandlingConfig.ResourceBaseName;
            var resourceBasePath = errorHandlingConfig.ResourceBasePath;

            var response = new List<ApplicationErrorTranslation>();
            var resources = Directory
                .GetFiles($"{resourceBasePath}")
                .Where(x => x.EndsWith(".resx") && !x.EndsWith($"{ resourceBaseName}.resx"))
                .ToList();


            foreach (var resource in resources)
            {
                var languageCode = resource.Split("\\")[1].Replace($"{resourceBaseName}.", "").Replace(".resx", "");
                XDocument xResx = null;
                xResx = XDocument.Load($"{resource}");
                if (xResx == null) continue;

                var xElement = xResx.Root.Descendants("data").Where(c => (string)c.Attribute("name") != null).ToList();
                var appErrorTranlationList = xElement
                .Select(x => new
                {
                    Name = x.FirstAttribute.Value,
                    Value = x?.Descendants("value")?.FirstOrDefault()?.Value ?? ""
                });

                response.AddRange(appErrorTranlationList.Select(x => new ApplicationErrorTranslation
                {
                    ErrorCode = long.Parse(x.Name),
                    ErrorDescription = x.Value,
                    LanguageCode = languageCode,
                    LanguageName = languageCode
                }).ToList());
            }
            Console.WriteLine($"=> Founded {resources.Count} resource files.{Environment.NewLine} {string.Join($"{Environment.NewLine} ", resources)}");
            Console.WriteLine($"=> Founded {response.Count} error keys.{Environment.NewLine} {string.Join($"{Environment.NewLine} ", response.Select(s => $"{s.ErrorCode}-{s.LanguageCode}"))}");

            return response;
        }
    }
}
