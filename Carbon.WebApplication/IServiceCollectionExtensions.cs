using Carbon.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Carbon.WebApplication
{

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds bearer authentication to the service
        /// </summary>
        /// <param name="services"> Specifies the contract for a collection of service descriptors.</param>
        /// <param name="configuration"> Represents a set of key/value application configuration properties.</param>
        /// <returns>Returns the contract for a collection of service descriptors.</returns>
        public static IServiceCollection AddBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (jwtSettings == null)
                throw new ArgumentNullException("JwtBearer Settings cannot be empty!");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = jwtSettings.Authority;
                        options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
                        options.Audience = jwtSettings.Audience;

                        if (jwtSettings.TokenValidationSettings != null)
                        {
                            var tokenValidationSettings = jwtSettings.TokenValidationSettings;
                            options.TokenValidationParameters.ValidIssuers = tokenValidationSettings.ValidIssuers;
                            options.TokenValidationParameters.ValidateIssuerSigningKey = tokenValidationSettings.ValidateIssuerSigningKey;
                        }
                    });

            return services;
        }
    }
}
