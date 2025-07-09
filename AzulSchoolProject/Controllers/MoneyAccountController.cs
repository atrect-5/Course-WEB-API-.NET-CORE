using Dtos.MoneyAccount;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.ComponentModel.DataAnnotations;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyAccountController(IMoneyAccountService moneyAccountService) : ControllerBase
    {
        private readonly IMoneyAccountService _moneyAccountService = moneyAccountService;

        /// <summary>
        /// Crea una nueva cuenta de dinero.
        /// </summary>
        /// <param name="model">El objeto con los datos para crear la nueva cuenta.</param>
        /// <returns>La cuenta de dinero recién creada.</returns>
        /// <response code="201">Retorna la cuenta recién creada y la URL para acceder a ella.</response>
        /// <response code="400">Si el objeto enviado es inválido.</response>
        [HttpPost]
        [ProducesResponseType(typeof(MoneyAccountDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMoneyAccountAsync([FromBody] CreateMoneyAccountDto model)
        {
            var newMoneyAccount = await _moneyAccountService.AddAsync(model);
            return CreatedAtAction(nameof(GetMoneyAccountByIdAsync), new { id = newMoneyAccount.Id }, newMoneyAccount );
        }

        /// <summary>
        /// Obtiene una cuenta de dinero específica por su ID.
        /// </summary>
        /// <param name="id">El ID de la cuenta a buscar.</param>
        /// <returns>La cuenta encontrada.</returns>
        /// <response code="200">Retorna la cuenta solicitada.</response>
        /// <response code="404">Si no se encuentra la cuenta con el ID especificado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MoneyAccountDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMoneyAccountByIdAsync(int id)
        {
            var moneyAccount = await _moneyAccountService.GetMoneyAccountByIdAsync(id);
            if (moneyAccount is null)
                return NotFound();

            return Ok(moneyAccount);
        }

        /// <summary>
        /// Obtiene una lista de cuentas de dinero para un usuario, con filtros opcionales.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se buscan las cuentas.</param>
        /// <param name="nameFilter">Filtro opcional para buscar cuentas por nombre.</param>
        /// <param name="typeFilter">Filtro opcional para buscar cuentas por tipo.</param>
        /// <returns>Una lista de cuentas que coinciden con los criterios.</returns>
        /// <response code="200">Retorna la lista de cuentas.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MoneyAccountDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMoneyAccountsByUserIdAsync([FromQuery][Required] int userId, [FromQuery] string? nameFilter = null, [FromQuery] string? typeFilter = null) =>
            Ok(await _moneyAccountService.GetMoneyAccountsByUserIdAsync(userId, nameFilter, typeFilter));

        /// <summary>
        /// Actualiza una cuenta de dinero existente.
        /// </summary>
        /// <param name="id">El ID de la cuenta a actualizar.</param>
        /// <param name="model">El objeto con los datos para actualizar la cuenta.</param>
        /// <returns>La cuenta actualizada.</returns>
        /// <response code="200">Retorna la cuenta con los datos actualizados.</response>
        /// <response code="400">Si los datos de la cuenta son inválidos.</response>
        /// <response code="404">Si no se encuentra la cuenta con el ID especificado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MoneyAccountDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMoneyAccountAsync(int id, [FromBody] UpdateMoneyAccountDto model)
        {
            var moneyAccount = await _moneyAccountService.UpdateAsync(id, model);
            if (moneyAccount is null)
                return NotFound();

            return Ok(moneyAccount);
        }

        /// <summary>
        /// Elimina una cuenta de dinero por su ID.
        /// </summary>
        /// <param name="id">El ID de la cuenta a eliminar.</param>
        /// <returns>No retorna contenido.</returns>
        /// <response code="204">Si la cuenta fue eliminada exitosamente.</response>
        /// <response code="404">Si no se encuentra la cuenta con el ID especificado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMoneyAccountAsync(int id)
        {
            if (await _moneyAccountService.DeleteAsync(id))
                return NoContent();

            return NotFound();
        }        
    }
}
