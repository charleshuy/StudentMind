using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace StudentMind.Razor.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the cookie exists
            var jwtToken = context.Request.Cookies["JWT_Token"];

            if (!string.IsNullOrEmpty(jwtToken))
            {
                // Add the token to the Authorization header
                context.Request.Headers["Authorization"] = $"Bearer {jwtToken}";
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

}
