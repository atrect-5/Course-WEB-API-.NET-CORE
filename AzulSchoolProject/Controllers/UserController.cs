using Dtos;
using Dtos.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="createUserDto">El objeto con los datos para crear el nuevo usuario.</param>
        /// <returns>El usuario recién creado.</returns>
        /// <response code="201">Retorna el usuario recién creado y la URL para acceder a él.</response>
        /// <response code="400">Si el objeto enviado es inválido.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var newUser = _userService.Add(createUserDto);

            // Return status 201 Created.
            // This includes the URI of the newly created resource in the Location header
            // Also includes the created user in the response body
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario a buscar.</param>
        /// <returns>El usuario encontrado.</returns>
        /// <response code="200">Retorna el usuario solicitado.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user is null)
                return NotFound(); // Return 404 Not Found if the user does not exist

            return Ok(user); // Return 200 OK with the user data
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios.
        /// </summary>
        /// <returns>Una lista de usuarios.</returns>
        /// <response code="200">Retorna la lista de todos los usuarios.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users); // Return 200 OK with the list of users
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="id">El ID del usuario a actualizar.</param>
        /// <param name="updateUserDto">El objeto con los datos para actualizar el usuario.</param>
        /// <returns>El usuario actualizado.</returns>
        /// <response code="200">Retorna el usuario con los datos actualizados.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = _userService.Update(id, updateUserDto);
            if (updatedUser is null)
                return NotFound();

            return Ok(updatedUser);
        }

        /// <summary>
        /// Elimina un usuario por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario a eliminar.</param>
        /// <returns>No retorna contenido.</returns>
        /// <response code="204">Si el usuario fue eliminado exitosamente.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteUser(int id) 
        {
            bool isDeleted = _userService.Delete(id);
            if (!isDeleted)
                return NotFound();
            return NoContent(); // 204 No Content
        }
    }
}
