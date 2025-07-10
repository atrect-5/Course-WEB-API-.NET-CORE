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
        /// <returns>Un token JWT si la autenticación es exitosa.</returns>
        /// <response code="200">Retorna el token JWT.</response>
        /// <response code="401">Si las credenciales son incorrectas.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login ([FromBody] LoginDto loginDto)
        {
            var token = await _authenticationService.AuthenticateAsync(loginDto.Email, loginDto.Password);

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Credenciales inválidas.");

            return Ok(new { Token = token });
        }
    }
}
