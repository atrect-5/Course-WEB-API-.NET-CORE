using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace AzulSchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyAccountController(IMoneyAccountService moneyAccountService) : ControllerBase
    {
        private readonly IMoneyAccountService _moneyAccountService = moneyAccountService;
    }
}
