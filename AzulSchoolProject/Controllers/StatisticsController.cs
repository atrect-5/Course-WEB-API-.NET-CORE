using AzulSchoolProject.Extensions;
using Dtos.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
    {
        private readonly IStatisticsService _statisticsService = statisticsService;

        /// <summary>
        /// Obtiene un desglose de gastos por categoría para un usuario en un período de tiempo.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se busca la estadística.</param>
        /// <param name="startDate">La fecha de inicio del período (formato: YYYY-MM-DD).</param>
        /// <param name="endDate">La fecha de fin del período (formato: YYYY-MM-DD).</param>
        /// <param name="limit">Opcional. Limita el resultado a las 'n' categorías con más gastos.</param>
        /// <returns>Una lista de categorías con el total gastado en cada una.</returns>
        /// <response code="200">Retorna el desglose de gastos.</response>
        /// <response code="400">Si las fechas no son válidas.</response>
        /// <response code="403">Si el usuario no tiene acceso a la ruta.</response>
        [HttpGet("{userId:int}/statistics/spending-by-category")]
        [ProducesResponseType(typeof(IEnumerable<CategorySummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpendingByCategory(int userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? limit)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId)
                return Forbid();

            // Se selecciona el ultimo mes si no se envia una fecha de inicio y fin
            var finalEndDate = endDate ?? DateTime.UtcNow;
            var finalStartDate = startDate ?? finalEndDate.AddMonths(-1);

            if (finalStartDate > finalEndDate)
                return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");

            var result = await _statisticsService.GetSummaryByCategoryAsync(userId, "EXPENDITURE", finalStartDate, finalEndDate, limit);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene un desglose de ingresos por categoría para un usuario en un período de tiempo.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se busca la estadística.</param>
        /// <param name="startDate">La fecha de inicio del período (formato: YYYY-MM-DD).</param>
        /// <param name="endDate">La fecha de fin del período (formato: YYYY-MM-DD).</param>
        /// <param name="limit">Opcional. Limita el resultado a las 'n' categorías con más ingresos.</param>
        /// <returns>Una lista de categorías con el total de ingresos en cada una.</returns>
        /// <response code="200">Retorna el desglose de gastos.</response>
        /// <response code="400">Si las fechas no son válidas.</response>
        /// <response code="403">Si el usuario no tiene acceso a la ruta.</response>
        [HttpGet("{userId:int}/statistics/income-by-category")]
        [ProducesResponseType(typeof(IEnumerable<CategorySummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIncomeByCategory(int userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? limit)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId)
                return Forbid();

            // Se selecciona el ultimo mes si no se envia una fecha de inicio y fin
            var finalEndDate = endDate ?? DateTime.UtcNow;
            var finalStartDate = startDate ?? finalEndDate.AddMonths(-1);

            if (finalStartDate > finalEndDate)
                return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin.");

            var result = await _statisticsService.GetSummaryByCategoryAsync(userId, "INCOME", finalStartDate, finalEndDate, limit);

            return Ok(result);
        }

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

            var summary = await _statisticsService.GetSummaryByUserIdAsync(userId);

            return summary is null ? NotFound() : Ok(summary);
        }

        /// <summary>
        /// Obtiene el flujo de efectivo mensual (ingresos vs. gastos) para un usuario.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se busca la estadística.</param>
        /// <param name="months">El número de meses a devolver (por defecto 6, máximo 24).</param>
        /// <returns>Una lista con el resumen de ingresos y gastos de cada mes.</returns>
        /// <response code="200">Retorna el flujo de efectivo mensual.</response>
        /// <response code="400">Si el número de meses es inválido.</response>
        /// <response code="403">Si el usuario no tiene permiso para ver estas estadísticas.</response>
        [HttpGet("{userId:int}/statistics/monthly-cash-flow")]
        [ProducesResponseType(typeof(IEnumerable<MonthlyFlowDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthlyCashFlow(int userId, [FromQuery] int months = 6)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId) return Forbid();

            if (months <= 0 || months > 36) return BadRequest("El número de meses debe estar entre 1 y 36");

            return Ok(await _statisticsService.GetMonthlyCashFlowAsync(userId, months));
        }
    }
}
