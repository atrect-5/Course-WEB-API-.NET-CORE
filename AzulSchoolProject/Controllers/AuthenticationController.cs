using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Services;
using Dtos.Authentication;
using AzulSchoolProject.Settings;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService, IAntiforgery antiforgery, CookieSettings cookieSettings, IRefreshTokenService refreshTokenService, ITokenService tokenService) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IAntiforgery _antiforgery = antiforgery;
        private readonly CookieSettings _cookieSettings = cookieSettings;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly ITokenService _tokenService = tokenService;

        /// <summary>
        /// Autentica a un usuario, setea cookie HttpOnly con JWT y cookie HttpOnly con refresh token; devuelve XSRF request token.
        /// </summary>
        /// <param name="loginDto">Las credenciales del usuario (email y contraseña).</param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login ([FromBody] LoginDto loginDto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].FirstOrDefault();

            var result = await _authenticationService.AuthenticateAsync(loginDto.Email, loginDto.Password, ip, userAgent, _cookieSettings.RefreshExpiresDays);

            if (result is null)
                return Unauthorized("Credenciales inválidas.");

            // JWT cookie
            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = _cookieSettings.AuthHttpOnly,
                Secure = _cookieSettings.AuthSecure,
                SameSite = _cookieSettings.AuthSameSite,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_cookieSettings.AuthExpiresMinutes),
                Path = "/"
            };
            Response.Cookies.Append(_cookieSettings.AuthCookieName, result.Value.Jwt, jwtCookieOptions);

            // Refresh cookie (HttpOnly)
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _cookieSettings.AuthSecure,
                SameSite = _cookieSettings.AuthSameSite,
                Expires = DateTimeOffset.UtcNow.Add(result.Value.RefreshExpiresAt - DateTime.UtcNow),
                Path = "/"
            };
            Response.Cookies.Append(_cookieSettings.RefreshCookieName, result.Value.RefreshToken, refreshCookieOptions);

            // Use antiforgery to create cookie token and return request token
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            return Ok(new { Message = "Autenticación exitosa.", XsrfToken = tokens.RequestToken });
        }

        /// <summary>
        /// Actualiza el JWT usando un refresh token.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue(_cookieSettings.RefreshCookieName, out var rawRefresh) || string.IsNullOrEmpty(rawRefresh))
                return Unauthorized("Missing refresh token.");

            var tokenRecord = await _refreshTokenService.GetByRawTokenAsync(rawRefresh);
            if (tokenRecord is null)
            {
                // Remove cookie if invalid
                Response.Cookies.Delete(_cookieSettings.RefreshCookieName, new CookieOptions { HttpOnly = true, Secure = _cookieSettings.AuthSecure, SameSite = _cookieSettings.AuthSameSite, Path = "/" });
                return Unauthorized("Invalid refresh token.");
            }

            if (tokenRecord.ExpiresAt <= DateTime.UtcNow || tokenRecord.RevokedAt != null)
            {
                // If expired or revoked, revoke all for user as a safety measure (optional)
                await _refreshTokenService.RevokeAllForUserAsync(tokenRecord.UserId);
                Response.Cookies.Delete(_cookieSettings.RefreshCookieName, new CookieOptions { HttpOnly = true, Secure = _cookieSettings.AuthSecure, SameSite = _cookieSettings.AuthSameSite, Path = "/" });
                return Unauthorized("Refresh token expired or revoked.");
            }

            // Rotate refresh token
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            var userAgent = Request.Headers["User-Agent"].FirstOrDefault();
            var (newToken, newExpires) = await _refreshTokenService.RotateAsync(tokenRecord, ip, userAgent, _cookieSettings.RefreshExpiresDays);

            // Create new JWT for the user attached to the token
            if (tokenRecord.User is null)
                return Unauthorized("User not found.");

            var newJwt = _tokenService.GenerateJWToken(tokenRecord.User);

            // Set cookies: JWT and new refresh
            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = _cookieSettings.AuthHttpOnly,
                Secure = _cookieSettings.AuthSecure,
                SameSite = _cookieSettings.AuthSameSite,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_cookieSettings.AuthExpiresMinutes),
                Path = "/"
            };
            Response.Cookies.Append(_cookieSettings.AuthCookieName, newJwt, jwtCookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _cookieSettings.AuthSecure,
                SameSite = _cookieSettings.AuthSameSite,
                Expires = DateTimeOffset.UtcNow.Add(newExpires - DateTime.UtcNow),
                Path = "/"
            };
            Response.Cookies.Append(_cookieSettings.RefreshCookieName, newToken, refreshCookieOptions);

            // Return new XSRF token
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new { Message = "Token refreshed.", XsrfToken = tokens.RequestToken });
        }

        /// <summary>
        /// Elimina la cookie de autenticación en logout y revoca refresh token en servidor.
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout()
        {
            // Attempt to revoke refresh token server-side
            Request.Cookies.TryGetValue(_cookieSettings.RefreshCookieName, out var rawRefresh);
            await _authenticationService.LogoutAsync(rawRefresh);

            // Delete cookie JWT
            Response.Cookies.Delete(_cookieSettings.AuthCookieName, new CookieOptions
            {
                HttpOnly = _cookieSettings.AuthHttpOnly,
                Secure = _cookieSettings.AuthSecure,
                SameSite = _cookieSettings.AuthSameSite,
                Path = "/"
            });

            // Delete antiforgery cookie (readable one)
            Response.Cookies.Delete(_cookieSettings.XsrfCookieName, new CookieOptions
            {
                HttpOnly = _cookieSettings.XsrfHttpOnly,
                Secure = _cookieSettings.XsrfSecure,
                SameSite = _cookieSettings.XsrfSameSite,
                Path = "/"
            });

            // Delete refresh cookie
            Response.Cookies.Delete(_cookieSettings.RefreshCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = _cookieSettings.AuthSecure,
                SameSite = _cookieSettings.AuthSameSite,
                Path = "/"
            });

            return NoContent();
        }
    }
}
