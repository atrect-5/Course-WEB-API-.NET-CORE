using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using AzulSchoolProject.Extensions;
using Common;
using Dtos.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> AddTransferAsync([FromBody] CreateTransferDto model)
        {
            var creatorId = User.GetUserId();
            var isCreatorAdmin = User.IsInRole("Admin");
            var result = await _transferService.AddAsync(model, creatorId, isCreatorAdmin);
            return result.IsSuccess
                ? CreatedAtRoute("GetTransferById", new { result.Value!.Id }, result.Value)
                : result.Status == Result.Forbidden
                    ? Forbid()
                    : NotFound();
        }

        /// <summary>
        /// Obtiene una transferencia específica por su ID.
        /// </summary>
        /// <param name="id">El ID de la transferencia a buscar.</param>
        /// <returns>La transferencia encontrada.</returns>
        /// <response code="200">Retorna la transferencia solicitada.</response>
        /// <response code="404">Si no se encuentra la transferencia con el ID especificado.</response>
        [HttpGet("{id}", Name = "GetTransferById")]
        [ProducesResponseType(typeof(TransferDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransferByIdAsync(int id)
        {
            var transfer = await _transferService.GetTransferByIdAsync(id);
            if (transfer is null)
                return NotFound();

            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (currentUserId != transfer.UserId && !isAdmin)
                return Forbid();

            return Ok(transfer);
        }

        /// <summary>
        /// Obtiene una lista de transferencias para un usuario, con filtros opcionales.
        /// </summary>
        /// <param name="userId">El ID del usuario para el que se buscan las transferencias (Solo para admins).</param>
        /// <param name="moneyAccountId">Filtro opcional para buscar transferencias por cuenta de dinero.</param>
        /// <param name="startDate">Filtro opcional para buscar transferencias desde una fecha.</param>
        /// <param name="endDate">Filtro opcional para buscar transferencias hasta una fecha.</param>
        /// <returns>Una lista de transferencias que coinciden con los criterios.</returns>
        /// <response code="200">Retorna la lista de transferencias.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransferDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransfersByUserIdAsync(
            [FromQuery] int? userId, [FromQuery] int? moneyAccountId = null, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var targetUserId = (isAdmin && userId.HasValue) ? userId.Value : currentUserId;

            return Ok(await _transferService.GetTransfersByUserIdAsync(
                targetUserId, moneyAccountId, startDate, endDate));
        }

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
        public async Task<IActionResult> DeleteTransferAsync(int id)
        {
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var result = await _transferService.DeleteAsync(id, currentUserId, isAdmin);

            return result.IsSuccess
                ? NoContent()
                : result.Status == Result.Forbidden
                    ? Forbid()
                    : NotFound();
        }
    }
}
