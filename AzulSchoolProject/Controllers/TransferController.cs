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
    }
}
