using System.Collections.Generic;

namespace Carbon.WebApplication
{
    public class JwtSettings
    {
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string Audience { get; set; }
        public TokenValidationSettings TokenValidationSettings { get; set; }
    }

    public class TokenValidationSettings
    {
        public IEnumerable<string> ValidIssuers { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
    }
}
