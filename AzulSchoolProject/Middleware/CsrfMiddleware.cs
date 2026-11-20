using AzulSchoolProject.Settings;
using Microsoft.AspNetCore.Antiforgery;

namespace AzulSchoolProject.Middleware
{
    public class CsrfMiddleware(RequestDelegate next, ClientSettings clientSettings, IAntiforgery antiforgery)
    {
        private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
        private readonly ClientSettings _clientSettings = clientSettings ?? throw new ArgumentNullException(nameof(clientSettings));
        private readonly IAntiforgery _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));

        private static bool IsExemptPath(string path) =>
            path.Equals("/api/authentication/login", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger/", StringComparison.OrdinalIgnoreCase);

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

            if (IsExemptPath(path))
            {
                await _next(context);
                return;
            }

            // Compute the API origin (useful for Swagger served from the same host)
            var apiOrigin = $"{req.Scheme}://{req.Host.Value}";

            // Read Origin/Referer 
            var origin = req.Headers["Origin"].FirstOrDefault() ?? req.Headers["Referer"].FirstOrDefault();

            var allowedOrigin = _clientSettings.AllowedFrontendOrigin ?? string.Empty;

            // Special case: allow user registration (POST /api/user) only if Origin matches allowed frontend OR apiOrigin
            if (HttpMethods.IsPost(req.Method) && path.Equals("/api/user", StringComparison.OrdinalIgnoreCase))
            {
                if (!IsAllowedOrigin(origin, allowedOrigin, apiOrigin))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Invalid origin for registration.");
                    return;
                }

                // Se puede agregar captcha aqu� si es necesario para prevenir spam de registro.
                await _next(context);
                return;
            }

            // Validate Origin or Referer header for other state-changing requests
            if (!IsAllowedOrigin(origin, allowedOrigin, apiOrigin))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid origin.");
                return;
            }

            // Delegate token validation to ASP.NET Core antiforgery
            try
            {
                await _antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Missing or invalid CSRF token.");
                return;
            }

            await _next(context);
        }

        private static bool IsAllowedOrigin(string? origin, string allowedFrontendOrigin, string apiOrigin) =>
            !string.IsNullOrWhiteSpace(origin) &&
            (HasSameOrigin(origin, allowedFrontendOrigin) || HasSameOrigin(origin, apiOrigin));

        private static bool HasSameOrigin(string candidate, string expected)
        {
            if (!Uri.TryCreate(candidate.Trim(), UriKind.Absolute, out var candidateUri) ||
                !Uri.TryCreate(expected, UriKind.Absolute, out var expectedUri) ||
                !IsHttpOrigin(candidateUri) ||
                !IsHttpOrigin(expectedUri) ||
                !string.IsNullOrEmpty(candidateUri.UserInfo))
            {
                return false;
            }

            return string.Equals(candidateUri.Scheme, expectedUri.Scheme, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(candidateUri.IdnHost, expectedUri.IdnHost, StringComparison.OrdinalIgnoreCase) &&
                   candidateUri.Port == expectedUri.Port;
        }

        private static bool IsHttpOrigin(Uri uri) =>
            uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
            uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
    }
}