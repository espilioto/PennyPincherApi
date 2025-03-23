using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Accounts.Models;

namespace PennyPincher.Web.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2")]
    public class AccountsController : ErrorOrApiController
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _accountService.GetAllAsyncV2();

            return result.Match(
                accounts => Ok(accounts),
                errors => Problem(errors)
            );
        }

        [HttpPost]
        public async Task<IActionResult> Post(AccountDto accountRequest)
        {
            throw new NotImplementedException();
        }
    }
}
