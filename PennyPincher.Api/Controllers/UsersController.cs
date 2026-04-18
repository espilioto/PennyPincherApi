using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Api.Extensions;
using PennyPincher.Contracts.Users;
using PennyPincher.Services.Users;

namespace PennyPincher.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ErrorOrApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

#if DEBUG
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _userService.GetAllAsync();

        return result.Match(
            users => Ok(users),
            errors => Problem(errors)
        );
    }
#endif

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);

        return result.Match(
            _ => Created(string.Empty, request),
            errors => Problem(errors)
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);

        return result.Match(
            response => Ok(response),
            errors => Problem(errors)
        );
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAccountRequest request)
    {
        var result = await _userService.DeleteAsync(User.GetUserId()!, request);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
