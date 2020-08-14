using Microsoft.AspNetCore.Builder;

namespace Carbon.WebApplication.Middlewares
{
    public static class BearerTokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseBearerTokenInRequestDto(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BearerTokenMiddleware>();
        }
    }
}
