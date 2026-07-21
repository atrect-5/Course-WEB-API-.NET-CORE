using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Dtos.Authentication;
using System.Threading.Tasks;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT.
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
                Secure = true, // ensure HTTPS in production
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60), // match token lifetime
                Path = "/"
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            return Ok(new { Message = "Autenticación exitosa." });
        }

        /// <summary>
        /// Elimina la cookie de autenticación en logout.
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout()
        {
            // Optional server-side cleanup
            //await _authenticationService.LogoutAsync();

            // Delete cookie
            Response.Cookies.Delete("AuthToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            return NoContent();
        }
    }
}
