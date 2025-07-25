﻿using AzulSchoolProject.Extensions;
using Dtos;
using Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
        {
            var newUser = await _userService.AddAsync(createUserDto);

            // Return status 201 Created.
            // This includes the URI of the newly created resource in the Location header
            // Also includes the created user in the response body
            return CreatedAtRoute("GetUserById", new { id = newUser.Id }, newUser);
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID.
        /// </summary>
        /// <param name="id">El ID del usuario a buscar.</param>
        /// <returns>El usuario encontrado.</returns>
        /// <response code="200">Retorna el usuario solicitado.</response>
        /// <response code="403">Si el usuario no tiene permiso para ver este recurso.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            if (!IsOwnerOrAdmin(id))
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            if (user is null)
                return NotFound(); // Return 404 Not Found if the user does not exist

            return Ok(user); // Return 200 OK with the user data
        }

        /// <summary>
        /// Obtiene una lista de todos los usuarios.
        /// </summary>
        /// <returns>Una lista de usuarios.</returns>
        /// <response code="200">Retorna la lista de todos los usuarios.</response>
        /// <response code="403">Si el usuario no es un administrador.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users); // Return 200 OK with the list of users
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="id">El ID del usuario a actualizar.</param>
        /// <param name="updateUserDto">El objeto con los datos para actualizar el usuario.</param>
        /// <returns>El usuario actualizado.</returns>
        /// <response code="200">Retorna el usuario con los datos actualizados.</response>
        /// <response code="403">Si el usuario no tiene permiso para actualizar este recurso.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!IsOwnerOrAdmin(id))
                return Forbid();

            var updatedUser = await _userService.UpdateAsync(id, updateUserDto);
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
        /// <response code="403">Si el usuario no tiene permiso para actualizar este recurso.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserAsync(int id) 
        {
            if (!IsOwnerOrAdmin(id))
                return Forbid();

            bool isDeleted = await _userService.DeleteAsync(id);
            if (!isDeleted)
                return NotFound();
            return NoContent(); // 204 No Content
        }

        private bool IsOwnerOrAdmin(int resourceId)
        {
            var currentUserId = User.GetUserId();
            // The user is authorized if they are the owner of the resource OR if they are an Admin.
            return currentUserId == resourceId || User.IsInRole("Admin");
        }
    }
}
