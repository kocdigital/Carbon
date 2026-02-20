using System.Collections.Generic;

namespace Carbon.WebApplication
{
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the Authority to use when making OpenIdConnect calls.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets if HTTPS is required for the metadata address or authority.
        /// The default is true. This should be disabled only in development environments.
        /// </summary>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// Gets or sets a single valid audience value for any received OpenIdConnect token.
        /// This value is passed into TokenValidationParameters.ValidAudience if that property is empty.
        /// </summary>
        /// <value>
        /// The expected audience for any received OpenIdConnect token.
        /// </value>
        public string Audience { get; set; }

        /// <summary>
        /// Token Validation Settings <see cref="TokenValidationSettings"/>
        /// </summary>
        public TokenValidationSettings TokenValidationSettings { get; set; }
        
        /// <summary>
        /// Token Version Validation Settings <see cref="TokenVersionValidationSettings"/>
        /// </summary>
        public TokenVersionValidationSettings TokenVersionValidation { get; set; }
    }

    public class TokenValidationSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="IEnumerable{String}"/> that contains valid issuers that will be used to check against the token's issuer.
        /// </summary>
        public IEnumerable<string> ValidIssuers { get; set; }

        /// <summary>
        /// Gets or sets a boolean that controls if validation of the securityKey that signed the securityToken is called.
        /// </summary>
        /// <remarks>It is possible for tokens to contain the public key needed to check the signature. For example, X509Data can be hydrated into an X509Certificate,
        /// which can be used to validate the signature. In these cases it is important to validate the SigningKey that was used to validate the signature. </remarks>
        public bool ValidateIssuerSigningKey { get; set; }
    }
    
    public class TokenVersionValidationSettings
    {
        /// <summary>
        /// Enables or disables token version validation.
        /// <para>
        /// When <c>false</c>, the validation step is skipped and JWT processing continues as usual.
        /// </para>
        /// </summary>
        public bool Enabled { get; set; } = false;
        
        /// <summary>
        /// The JWT claim name that carries the token version value.
        /// <para>
        /// Example: <c>TokenVersion</c>.
        /// </para>
        /// </summary>
        public string TokenVersionClaimName { get; set; } = "TokenVersion";
        
        /// <summary>
        /// The JWT claim name that carries the tenant identifier.
        /// <para>
        /// Used to build the Redis key in multi-tenant environments.
        /// Example: <c>tenant-id</c>.
        /// </para>
        /// </summary>
        public string TenantIdClaimName { get; set; } = "tenant-id";
        
        /// <summary>
        /// The JWT claim name that carries the user identifier.
        /// <para>
        /// For IdentityServer-based tokens, this is typically <c>sub</c>.
        /// </para>
        /// </summary>
        public string UserIdClaimName { get; set; } = "sub";
        
        /// <summary>
        /// Prefix/namespace used specifically for IdentityServer-issued auth keys stored in Redis.
        /// Example: "IdentityInstance".
        /// </summary>
        public string IdentityInstance { get; set; } = "IdentityInstance";
        
        /// <summary>
        /// Redis key template used to fetch the current token version.
        /// <para>
        /// Placeholders:
        /// <list type="bullet">
        /// <item><description><c>{identityInstance} - namespace/prefix from IdentityInstance (IdentityServer keys)</description></item>
        /// <item><description><c>{tenantId}</c> - tenant identifier from <see cref="TenantIdClaimName"/></description></item>
        /// <item><description><c>{userId}</c> - user identifier from <see cref="UserIdClaimName"/></description></item>
        /// </list>
        /// Example: <c>{identityInstance}:auth:TokenVersion:{tenantId}:{userId}</c>.
        /// </para>
        /// </summary>
        public string RedisKeyFormat { get; set; } = "{identityInstance}:auth:TokenVersion:{tenantId}:{userId}";
        
        /// <summary>
        /// Determines behavior when the Redis key does not exist or cannot be parsed.
        /// <para>
        /// When <c>true</c>, missing/invalid Redis data causes authentication to fail (401).
        /// When <c>false</c>, validation is skipped in that case.
        /// </para>
        /// </summary>
        public bool FailIfRedisMissing { get; set; } = true;
        
        /// <summary>
        /// Determines whether to skip validation when Redis is disabled in the application.
        /// <para>
        /// In Carbon setups where Redis can be replaced by a dummy multiplexer, setting this to <c>true</c> prevents
        /// token validation from failing due to Redis being intentionally turned off.
        /// </para>
        /// </summary>
        public bool SkipWhenRedisDisabled { get; set; } = true;

        /// <summary>
        /// When true, skips token version validation if Redis services are not registered (IDatabase/IConnectionMultiplexer not found).
        /// Useful for APIs that do not use Redis even if Redis is enabled globally.
        /// </summary>
        public bool SkipIfRedisNotRegistered { get; set; } = false;
    }
}
