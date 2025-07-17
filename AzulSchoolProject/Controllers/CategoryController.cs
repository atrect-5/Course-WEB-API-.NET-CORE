using AzulSchoolProject.Extensions;
using Common;
using Dtos.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AzulSchoolProject.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="createCategoryDto">El objeto con los datos para crear la nueva categoría (El tipo de categoria "Type" debe ser "INCOME" o "EXPENDITURE").</param>
        /// <returns>La categoría recién creada.</returns>
        /// <response code="201">Retorna la categoría recién creada y la URL para acceder a ella.</response>
        /// <response code="400">Si el objeto enviado es inválido (ej. tipo de categoría incorrecto).</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDto createCategoryDto)
        {
            var creatorId = User.GetUserId();
            var isCreatorAdmin = User.IsInRole("Admin");
            var newCategory = await _categoryService.AddAsync(createCategoryDto, creatorId, isCreatorAdmin);
            return CreatedAtRoute("GetCategoryById", new { id = newCategory.Id }, newCategory);
        }

        /// <summary>
        /// Obtiene una categoria específica por su ID.
        /// </summary>
        /// <param name="id">El ID de la categoria a buscar.</param>
        /// <returns>La categoria encontrada.</returns>
        /// <response code="200">Retorna la categoria solicitada.</response>
        /// <response code="403">Si el usuario no tiene permiso para ver este recurso.</response>
        /// <response code="404">Si no se encuentra la categoria con el ID especificado.</response>
        [HttpGet("{id}", Name = "GetCategoryById")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
                return NotFound(); // Return 404 Not Found if the category does not exist

            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            // Un usuario puede ver una categoría si:
            // 1. Es un administrador.
            // 2. La categoría es global (no tiene dueño).
            // 3. La categoría le pertenece.
            if (!isAdmin && category.UserId != null && category.UserId != currentUserId)
                return Forbid(); // Return 403 Forbidden if the user does not have permission to view this category

            return Ok(category); // Return 200 OK with the category Dto
        }

        /// <summary>
        /// Obtiene una lista de categorías para un usuario, con filtros opcionales.
        /// </summary>
        /// <param name="userId">ID del usuario para el que se listan las categorías. Solo para administradores.</param>
        /// <param name="nameFilter">Filtro opcional para buscar categorías por nombre (no sensible a mayúsculas/minúsculas).</param>
        /// <param name="typeFilter">Filtro opcional para buscar categorías por tipo (INCOME o EXPENDITURE).</param>
        /// <returns>Una lista de categorías que coinciden con los criterios.</returns>
        /// <response code="200">Retorna la lista de categorías.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesByUserIdAsync([FromQuery] int? userId,[FromQuery] string? nameFilter = null, [FromQuery] string? typeFilter = null)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var targetUserId = (isAdmin && userId.HasValue) ? userId.Value : currentUserId;

            var categories = await _categoryService.GetCategoriesByUserIdAsync(targetUserId, nameFilter, typeFilter);
            return Ok(categories); // Return 200 OK with the list of categories
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="id">El ID de la categoría a actualizar.</param>
        /// <param name="updateCategoryDto">El objeto con los datos para actualizar la categoría.</param>
        /// <returns>La categoría actualizada.</returns>
        /// <response code="200">Retorna la categoría con los datos actualizados.</response>
        /// <response code="400">Si los datos de la categoría son inválidos.</response>
        /// <response code="403">Si el usuario no tiene permiso para ver este recurso.</response>
        /// <response code="404">Si no se encuentra la categoría con el ID especificado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            ArgumentNullException.ThrowIfNull(updateCategoryDto);
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _categoryService.UpdateAsync(id, updateCategoryDto, userId, isAdmin);


            return result.IsSuccess
                ? Ok(result.Value)
                : result.Status == Result.NotFound
                    ? NotFound()
                    : Forbid();
        }

        /// <summary>
        /// Elimina una categoría por su ID.
        /// </summary>
        /// <param name="id">El ID de la categoría a eliminar.</param>
        /// <returns>No retorna contenido.</returns>
        /// <response code="204">Si la categoría fue eliminada exitosamente.</response>
        /// <response code="403">Si el usuario no tiene permiso para ver este recurso.</response>
        /// <response code="404">Si no se encuentra la categoría con el ID especificado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _categoryService.DeleteAsync(id, userId, isAdmin);

            return result.IsSuccess
                ? NoContent()
                : result.Status == Result.NotFound
                    ? NotFound()
                    : Forbid();
        }
    }
}
