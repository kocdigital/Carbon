using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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

						// Preserve original events behavior:
                        // - If caller supplies events, use that as the base.
                        // - Else if options.Events already has something, keep it.
                        // - Else create a new JwtBearerEvents.
                        var baseEvents = events ?? options.Events as JwtBearerEvents ?? new JwtBearerEvents();

                        // If TokenVersionValidation is enabled, wrap ONLY OnTokenValidated.
                        var tvs = jwtSettings.TokenVersionValidation;
                        if (tvs != null && tvs.Enabled)
                        {
                            var previousOnTokenValidated = baseEvents.OnTokenValidated;

                            baseEvents.OnTokenValidated = async ctx =>
                            {
                                // 1) Run existing handler first (if any)
                                if (previousOnTokenValidated != null)
                                    await previousOnTokenValidated(ctx);

                                // If previous handler failed auth, do not proceed.
                                if (ctx.Result?.Failure != null)
                                    return;

                                // 2) Skip validation when Redis is intentionally disabled (Dummy) OR Redis not registered (optional)
                                IConnectionMultiplexer mux = null;
                                if (tvs.SkipWhenRedisDisabled || tvs.SkipIfRedisNotRegistered)
                                {
                                    mux = ctx.HttpContext.RequestServices.GetService<IConnectionMultiplexer>();

                                    // If Redis is not registered in this API and we want to skip in that case, skip.
                                    if (mux == null && tvs.SkipIfRedisNotRegistered)
                                        return;

                                    // If dummy multiplexer is used (Redis disabled) and we want to skip, skip.
                                    if (tvs.SkipWhenRedisDisabled &&
                                        mux != null &&
                                        mux.GetType().Name.Contains("DummyConnectionMultiplexer", StringComparison.OrdinalIgnoreCase))
                                    {
                                        return;
                                    }
                                }

                                // 3) Resolve Redis
                                var redisDb = ctx.HttpContext.RequestServices.GetService<IDatabase>();
                                if (redisDb == null)
                                {
                                    // If Redis is not registered in this API and we want to skip in that case, skip.
                                    if (tvs.SkipIfRedisNotRegistered)
                                        return;

                                    ctx.Fail("Redis is not available.");
                                    return;
                                }

                                // 4) Read claims (tenantId, userId, tokenVersion)
                                var principal = ctx.Principal;

                                var tenantId = principal?.FindFirst(tvs.TenantIdClaimName)?.Value;
                                var userId = principal?.FindFirst(tvs.UserIdClaimName)?.Value;

                                if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId))
                                {
                                    ctx.Fail("Missing tenantId/userId claim.");
                                    return;
                                }

                                var tokenVersionStr = principal?.FindFirst(tvs.TokenVersionClaimName)?.Value;
                                if (!long.TryParse(tokenVersionStr, out var tokenVersionFromJwt))
                                {
                                    ctx.Fail("Missing/invalid token version claim.");
                                    return;
                                }

                                // 5) Build Redis key
                                var key = BuildTokenVersionKey(tvs, tenantId, userId);

                                // 6) Fetch current version from Redis
                                RedisValue redisValue = await redisDb.StringGetAsync(key);
                                var currentStr = redisValue.HasValue ? redisValue.ToString() : null;

                                if (!long.TryParse(currentStr, out var currentVersion))
                                {
                                    if (tvs.FailIfRedisMissing)
                                    {
                                        ctx.Fail("Token version not found.");
                                        return;
                                    }

                                    // lenient mode: skip if missing/unparseable
                                    return;
                                }

                                // 7) Compare
                                if (currentVersion != tokenVersionFromJwt)
                                {
                                    ctx.Fail("Token revoked.");
                                    return;
                                }
                            };
                        }

                        // Set back base events (may be the caller's object, now wrapped)
                        options.Events = baseEvents;
                    });

			return services;
		}
        
		private static string BuildTokenVersionKey(TokenVersionValidationSettings tvs, string tenantId, string userId)
		{
			var identityInstance = string.IsNullOrWhiteSpace(tvs.IdentityInstance) ? "IdentityInstance" : tvs.IdentityInstance;

			return tvs.RedisKeyFormat
				.Replace("{identityInstance}", identityInstance, StringComparison.OrdinalIgnoreCase)
				.Replace("{tenantId}", tenantId ?? "", StringComparison.OrdinalIgnoreCase)
				.Replace("{userId}", userId ?? "", StringComparison.OrdinalIgnoreCase);
		}
	}
}
