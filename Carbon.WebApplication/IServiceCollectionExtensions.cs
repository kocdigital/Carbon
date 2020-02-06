using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Carbon.WebApplication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT").Get<JWTSettings>();

            if (jwtSettings == null)
                throw new ArgumentNullException("JWT Settings cannot be empty!");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = jwtSettings.Authority;
                        options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
                        options.Audience = jwtSettings.Audience;
                    });

            return services;
        }
    }
}
