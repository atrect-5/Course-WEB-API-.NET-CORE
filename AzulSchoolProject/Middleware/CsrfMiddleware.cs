
namespace AzulSchoolProject.Middleware
{
    public class CsrfMiddleware(RequestDelegate next, string allowedOrigin)
    {
        private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
        private readonly string _allowedOrigin = allowedOrigin ?? throw new ArgumentNullException(nameof(allowedOrigin));
        // paths to skip CSRF validation (login/logout, swagger, static files, etc.)
        private static readonly string[] _exemptPaths = new[]
        {
            "/api/authentication/login",
            "/api/authentication/logout",
            "/swagger",
            "/swagger/index.html",
            "/swagger/v1/swagger.json"
        };

        public async Task InvokeAsync(HttpContext context)
        {
            var req = context.Request;
            var path = req.Path.Value ?? string.Empty;

            // Skip safe methods and OPTIONS
            if (HttpMethods.IsGet(req.Method) ||
                HttpMethods.IsHead(req.Method) ||
                HttpMethods.IsOptions(req.Method) ||
                HttpMethods.IsTrace(req.Method))
            {
                await _next(context);
                return;
            }

            // Skip exempt paths
            if (_exemptPaths.Any(p => !string.IsNullOrEmpty(p) && path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Compute the API origin (useful for Swagger served from the same host)
            var apiOrigin = $"{req.Scheme}://{req.Host.Value}";

            // Read Origin/Referer 
            var origin = req.Headers["Origin"].FirstOrDefault() ?? req.Headers["Referer"].FirstOrDefault();

            // Special case: allow user registration (POST /api/users) only if Origin matches allowed frontend OR apiOrigin
            if (HttpMethods.IsPost(req.Method) && path.StartsWith("/api/user", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(origin) ||
                    !(origin.StartsWith(_allowedOrigin, StringComparison.OrdinalIgnoreCase) ||
                      origin.StartsWith(apiOrigin, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Invalid origin for registration.");
                    return;
                }

                // Se puede agregar captcha aquí si es necesario para prevenir spam de registro.
                await _next(context);
                return;
            }

            // Validate Origin or Referer header for other state-changing requests
            if (string.IsNullOrEmpty(origin) ||
                !(origin.StartsWith(_allowedOrigin, StringComparison.OrdinalIgnoreCase) ||
                  origin.StartsWith(apiOrigin, StringComparison.OrdinalIgnoreCase) ||
                  origin.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase) || // allow local dev
                  origin.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase)))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid origin.");
                return;
            }

            // Validate double-submit: header vs cookie
            req.Cookies.TryGetValue("XSRF-TOKEN", out var cookieToken);
            var headerToken = req.Headers["X-CSRF-TOKEN"].FirstOrDefault();

            if (string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(headerToken) ||
                !string.Equals(cookieToken, headerToken, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Missing or invalid CSRF token.");
                return;
            }

            await _next(context);
        }
    }
}