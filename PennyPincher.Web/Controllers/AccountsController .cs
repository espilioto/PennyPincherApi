using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Services.Accounts;

namespace PennyPincher.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ErrorOrApiController
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _accountService.GetAllAsync();

        return result.Match(
            accounts => Ok(accounts),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post(AccountRequest request)
    {
        var result = await _accountService.InsertAsync(request);

        return result.Match(
            account => Created(string.Empty, account),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int accountId, [FromBody] AccountRequest request)
    {
        var result = await _accountService.UpdateAsync(accountId, request);

        return result.Match(
          _ => Ok(),
          errors => Problem(errors)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _accountService.DeleteAsync(id);

        return result.Match(
         _ => NoContent(),
         errors => Problem(errors)
     );
    }
}
