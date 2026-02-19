using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Carbon.WebApplication
{
    public class TokenVersionValidator
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenVersionValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ValidateAsync(TokenValidatedContext ctx, TokenVersionValidationSettings tvs)
        {
            IConnectionMultiplexer mux = null;
            if (tvs.SkipWhenRedisDisabled || tvs.SkipIfRedisNotRegistered)
            {
                mux = _serviceProvider.GetService<IConnectionMultiplexer>();
                
                if (mux == null && tvs.SkipIfRedisNotRegistered)
                    return;
                
                if (tvs.SkipWhenRedisDisabled &&
                    mux != null &&
                    mux.GetType().Name.Contains("DummyConnectionMultiplexer",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
            
            var redisDb = _serviceProvider.GetService<IDatabase>();
            if (redisDb == null)
            {
                if (tvs.SkipIfRedisNotRegistered)
                    return;

                ctx.Fail("Redis is not available.");
                return;
            }
            
            var principal = ctx.Principal;

            var tenantId = principal?.FindFirst(tvs.TenantIdClaimName)?.Value;
            var userId = principal?.FindFirst(tvs.UserIdClaimName)?.Value;

            if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId))
            {
                ctx.Fail("Missing tenantId/userId claim.");
                return;
            }

            var tokenVersionClaim = principal?.FindFirst(tvs.TokenVersionClaimName);
            
            if (tokenVersionClaim == null)
            {
                return;
            }
            
            if (!long.TryParse(tokenVersionClaim.Value, out var tokenVersionFromJwt))
            {
                ctx.Fail("Invalid token version claim format.");
                return;
            }
            
            var key = BuildTokenVersionKey(tvs, tenantId, userId);
            
            RedisValue redisValue = await redisDb.StringGetAsync(key);
            var currentStr = redisValue.HasValue ? redisValue.ToString() : null;

            if (!long.TryParse(currentStr, out var currentVersion))
            {
                if (tvs.FailIfRedisMissing)
                {
                    ctx.Fail("Token version not found.");
                    return;
                }
                
                return;
            }
            
            if (currentVersion != tokenVersionFromJwt)
            {
                ctx.Fail("Token revoked.");
                return;
            }
        }

        private string BuildTokenVersionKey(TokenVersionValidationSettings tvs, string tenantId, string userId)
        {
            var identityInstance = string.IsNullOrWhiteSpace(tvs.IdentityInstance)
                ? "IdentityInstance"
                : tvs.IdentityInstance;

            return tvs.RedisKeyFormat
                .Replace("{identityInstance}", identityInstance, StringComparison.OrdinalIgnoreCase)
                .Replace("{tenantId}", tenantId ?? "", StringComparison.OrdinalIgnoreCase)
                .Replace("{userId}", userId ?? "", StringComparison.OrdinalIgnoreCase);
        }
    }
}