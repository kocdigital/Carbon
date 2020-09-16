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
}
