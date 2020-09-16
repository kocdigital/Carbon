using Microsoft.AspNetCore.Builder;

namespace Carbon.WebApplication.Middlewares
{
    public static class BearerTokenMiddlewareExtensions
    {
        /// <summary>
        /// Adds a middleware type to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseBearerTokenInRequestDto(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BearerTokenMiddleware>();
        }
    }
}
