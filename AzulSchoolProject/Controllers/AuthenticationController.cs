using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Services;
using Dtos.Authentication;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService, IAntiforgery antiforgery) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IAntiforgery _antiforgery = antiforgery;

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
                HttpOnly = true,
                Secure = true, // HTTPS required when SameSite=None
                SameSite = SameSiteMode.None, // allow cross-site requests
                Expires = DateTimeOffset.UtcNow.AddMinutes(60), // match token lifetime
                Path = "/"
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

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
            Response.Cookies.Delete("AuthToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            // Delete antiforgery cookie (readable one)
            Response.Cookies.Delete("XSRF-TOKEN", new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Task.FromResult<IActionResult>(NoContent());
        }
    }
}
