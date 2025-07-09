using System.ComponentModel.DataAnnotations;
using Dtos.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController(ITransactionService transactionService) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;

        /// <summary>
        /// Crea una nueva transacción.
        /// </summary>
        /// <param name="model">El objeto con los datos para crear la nueva transacción.</param>
        /// <returns>La transacción recién creada.</returns>
        /// <response code="201">Retorna la transacción recién creada y la URL para acceder a ella.</response>
        /// <response code="400">Si el objeto enviado es inválido.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTransactionAsync([FromBody] CreateTransactionDto model)
        {
            var newTransaction = await _transactionService.AddAsync(model);
            return CreatedAtAction(nameof(GetTransactionByIdAsync), new { id = newTransaction.Id }, newTransaction);
        }

        /// <summary>
        /// Obtiene una transacción específica por su ID.
        /// </summary>
        /// <param name="id">El ID de la transacción a buscar.</param>
        /// <returns>La transacción encontrada.</returns>
        /// <response code="200">Retorna la transacción solicitada.</response>
        /// <response code="404">Si no se encuentra la transacción con el ID especificado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction is null)
                return NotFound();
            return Ok(transaction);
        }

        /// <summary>
        /// Obtiene una lista de transacciones para un usuario, con filtros opcionales.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se buscan las transacciones.</param>
        /// <param name="moneyAccountId">Filtro opcional para buscar transacciones por cuenta de dinero.</param>
        /// <param name="categoryId">Filtro opcional para buscar transacciones por categoría.</param>
        /// <param name="startDate">Filtro opcional para buscar transacciones desde una fecha.</param>
        /// <param name="endDate">Filtro opcional para buscar transacciones hasta una fecha.</param>
        /// <returns>Una lista de transacciones que coinciden con los criterios.</returns>
        /// <response code="200">Retorna la lista de transacciones.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionsByUserIdAsync(
            [FromQuery] [Required] int userId, [FromQuery] int? moneyAccountId = null, [FromQuery] int? categoryId = null,
            [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) =>
            Ok(await _transactionService.GetTransactionsByUserIdAsync(
                userId, moneyAccountId, categoryId, startDate, endDate));

        /// <summary>
        /// Actualiza una transacción existente.
        /// </summary>
        /// <param name="id">El ID de la transacción a actualizar.</param>
        /// <param name="model">El objeto con los datos para actualizar la transacción.</param>
        /// <returns>La transacción actualizada.</returns>
        /// <response code="200">Retorna la transacción con los datos actualizados.</response>
        /// <response code="400">Si los datos de la transacción son inválidos.</response>
        /// <response code="404">Si no se encuentra la transacción con el ID especificado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTransactionAsync(int id, [FromBody] UpdateTransactionDto model)
        {
            var updatedTransaction = await _transactionService.UpdateAsync(id, model);
            if (updatedTransaction is null)
                return NotFound();
            return Ok(updatedTransaction);
        }

        /// <summary>
        /// Elimina una transacción por su ID.
        /// </summary>
        /// <param name="id">El ID de la transacción a eliminar.</param>
        /// <returns>No retorna contenido.</returns>
        /// <response code="204">Si la transacción fue eliminada exitosamente.</response>
        /// <response code="404">Si no se encuentra la transacción con el ID especificado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            if (await _transactionService.DeleteAsync(id))
                return NoContent();
            return NotFound();
        }
    }
}
