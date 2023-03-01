using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Accounts.Models;

namespace PennyPincher.Web.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IEnumerable<AccountDto>> Get()
        {
            var accounts = await _accountService.GetAllAsync();
            return accounts;
        }

        [HttpPost]
        public async Task<IActionResult> Post(AccountDto accountRequest)
        {
            var result = await _accountService.InsertAsync(accountRequest);

            return result ? Created(string.Empty, result) : StatusCode(500);
        }
    }
}
