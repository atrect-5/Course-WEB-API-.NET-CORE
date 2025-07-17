using AzulSchoolProject.Extensions;
using Dtos.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class FinancialSummaryController(IFinancialSummaryService summaryService) : ControllerBase
    {
        private readonly IFinancialSummaryService _summaryService = summaryService;

        /// <summary>
        /// Obtiene un resumen financiero para un usuario.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se busca el resumen.</param>
        /// <returns>Un resumen de los balances y créditos del usuario.</returns>
        /// <response code="200">Retorna el resumen financiero.</response>
        /// <response code="403">Si el usuario no tiene permiso de acceder a esta ruta.</response>
        /// <response code="404">Si no se encuentra el usuario con el ID especificado.</response>
        [HttpGet("{userId:int}/summary")]
        [ProducesResponseType(typeof(FinancialSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFinancialSummary(int userId)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId) return Forbid();

            var summary = await _summaryService.GetSummaryByUserIdAsync(userId);

            return summary is null ? NotFound() : Ok(summary);
        }
    }
}
