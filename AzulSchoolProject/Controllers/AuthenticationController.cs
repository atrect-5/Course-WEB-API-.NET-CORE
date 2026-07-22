using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Services;
using Dtos.Authentication;
using AzulSchoolProject.Settings;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService, IAntiforgery antiforgery, CookieSettings cookieSettings) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IAntiforgery _antiforgery = antiforgery;
        private readonly CookieSettings _cookieSettings = cookieSettings;

        /// <summary>
        /// Autentica a un usuario, setea cookie HttpOnly con JWT y devuelve XSRF request token.
        /// </summary>
        /// <param name="loginDto">Las credenciales del usuario (email y contraseña).</param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login ([FromBody] LoginDto loginDto)
        {
            var token = await _authenticationService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Credenciales inválidas.");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = _cookieSettings.AuthHttpOnly,
                Secure = _cookieSettings.AuthSecure, 
                SameSite = _cookieSettings.AuthSameSite, 
                Expires = DateTimeOffset.UtcNow.AddMinutes(_cookieSettings.AuthExpiresMinutes), 
                Path = "/"
            };

            Response.Cookies.Append(_cookieSettings.AuthCookieName, token, cookieOptions);

            // Use antiforgery to create cookie token and return request token
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

            return Ok(new { Message = "Autenticación exitosa.", XsrfToken = tokens.RequestToken });
        }

        /// <summary>
        /// Elimina la cookie de autenticación en logout.
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public Task<IActionResult> Logout()
        {
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

            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}
