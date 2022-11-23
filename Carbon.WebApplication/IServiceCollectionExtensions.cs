using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Carbon.WebApplication
{
    /// <summary>
    /// Contains extension methods for <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
	{
        /// <summary>
        /// Adds bearer authentication to the service
        /// <para>
        /// Uses configuration parameter's "JwtSettings" key and binds that configuration to <see cref="JwtSettings"/> instance.
        /// </para>
        /// </summary>
        /// <param name="services"> Specifies the contract for a collection of service descriptors.</param>
        /// <param name="configuration"> Represents a set of key/value application configuration properties.</param>
        /// <returns>Returns the contract for a collection of service descriptors.</returns>
		/// <exception cref="System.ArgumentNullException">Throws if JwtSettings is empty.</exception>
        public static IServiceCollection AddBearerAuthentication(this IServiceCollection services, IConfiguration configuration, JwtBearerEvents events = null)
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

						if(events != null)
                        {
							options.Events = events;
                        }
					});

			return services;
		}
	}
}
