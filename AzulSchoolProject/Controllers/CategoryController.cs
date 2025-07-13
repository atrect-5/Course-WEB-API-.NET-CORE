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
        /// <param name="createCategoryDto">El objeto con los datos para crear la nueva categoría.</param>
        /// <returns>La categoría recién creada.</returns>
        /// <response code="201">Retorna la categoría recién creada y la URL para acceder a ella.</response>
        /// <response code="400">Si el objeto enviado es inválido (ej. tipo de categoría incorrecto).</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryDto createCategoryDto)
        {
            var newCategory = await _categoryService.AddAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = newCategory.Id }, newCategory);
        }

        /// <summary>
        /// Obtiene una categoria específica por su ID.
        /// </summary>
        /// <param name="id">El ID de la categoria a buscar.</param>
        /// <returns>La categoria encontrada.</returns>
        /// <response code="200">Retorna la categoria solicitada.</response>
        /// <response code="404">Si no se encuentra la categoria con el ID especificado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
                return NotFound(); // Return 404 Not Found if the category does not exist

            return Ok(category); // Return 200 OK with the category Dto
        }

        /// <summary>
        /// Obtiene una lista de categorías para un usuario, con filtros opcionales.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se buscan las categorías.</param>
        /// <param name="nameFilter">Filtro opcional para buscar categorías por nombre (no sensible a mayúsculas/minúsculas).</param>
        /// <param name="typeFilter">Filtro opcional para buscar categorías por tipo (INCOME o EXPENDITURE).</param>
        /// <returns>Una lista de categorías que coinciden con los criterios.</returns>
        /// <response code="200">Retorna la lista de categorías.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesByUserIdAsync([FromQuery][Required] int userId, [FromQuery] string? nameFilter = null, [FromQuery] string? typeFilter = null)
        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId, nameFilter, typeFilter);
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
        /// <response code="404">Si no se encuentra la categoría con el ID especificado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            ArgumentNullException.ThrowIfNull(updateCategoryDto);
            var updatedCategory = await _categoryService.UpdateAsync(id, updateCategoryDto);
            if (updatedCategory is null)
                return NotFound(); // Return 404 Not Found if the category to update does not exist

            return Ok(updatedCategory); // Return 200 OK with the updated category Dto
        }

        /// <summary>
        /// Elimina una categoría por su ID.
        /// </summary>
        /// <param name="id">El ID de la categoría a eliminar.</param>
        /// <returns>No retorna contenido.</returns>
        /// <response code="204">Si la categoría fue eliminada exitosamente.</response>
        /// <response code="404">Si no se encuentra la categoría con el ID especificado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            if (await _categoryService.DeleteAsync(id))
                return NoContent(); // Return 204 No Content if the category was successfully deleted

            return NotFound(); // Return 404 Not Found if the category to delete does not exist
        }
    }
}
