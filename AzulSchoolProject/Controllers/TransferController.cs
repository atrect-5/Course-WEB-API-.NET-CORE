using System.ComponentModel.DataAnnotations;
using Dtos.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController(ITransferService transferService) : ControllerBase
    {
        private readonly ITransferService _transferService = transferService;

        /// <summary>
        /// Crea una nueva transferencia.
        /// </summary>
        /// <param name="model">El objeto con los datos para crear la nueva transferencia.</param>
        /// <returns>La transferencia recién creada.</returns>
        /// <response code="201">Retorna la transferencia recién creada y la URL para acceder a ella.</response>
        /// <response code="400">Si el objeto enviado es inválido.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransferDto), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddTransfer([FromBody] CreateTransferDto model)
        {
            var newTransfer = _transferService.Add(model);
            return CreatedAtAction(nameof(GetTransferById), new { id = newTransfer.Id }, newTransfer);
        }

        /// <summary>
        /// Obtiene una transferencia específica por su ID.
        /// </summary>
        /// <param name="id">El ID de la transferencia a buscar.</param>
        /// <returns>La transferencia encontrada.</returns>
        /// <response code="200">Retorna la transferencia solicitada.</response>
        /// <response code="404">Si no se encuentra la transferencia con el ID especificado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransferDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTransferById(int id)
        {
            var transfer = _transferService.GetTransferById(id);
            if (transfer is null)
                return NotFound();
            return Ok(transfer);
        }

        /// <summary>
        /// Obtiene una lista de transferencias para un usuario, con filtros opcionales.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se buscan las transferencias.</param>
        /// <param name="moneyAccountId">Filtro opcional para buscar transferencias por cuenta de dinero.</param>
        /// <param name="startDate">Filtro opcional para buscar transferencias desde una fecha.</param>
        /// <param name="endDate">Filtro opcional para buscar transferencias hasta una fecha.</param>
        /// <returns>Una lista de transferencias que coinciden con los criterios.</returns>
        /// <response code="200">Retorna la lista de transferencias.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransferDto>), StatusCodes.Status200OK)]
        public IActionResult GetTransfersByUserId(
            [FromQuery] [Required] int userId, [FromQuery] int? moneyAccountId = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) =>
            Ok(_transferService.GetTransfersByUserId(
                userId, moneyAccountId, startDate, endDate));

        /// <summary>
        /// Elimina una transferencia por su ID.
        /// </summary>
        /// <param name="id">El ID de la transferencia a eliminar.</param>
        /// <returns>No retorna contenido.</returns>
        /// <response code="204">Si la transferencia fue eliminada exitosamente.</response>
        /// <response code="404">Si no se encuentra la transferencia con el ID especificado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteTransfer(int id)
        {
            if (_transferService.Delete(id))
                return NoContent();
            return NotFound();
        }
    }
}
