using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements;
using PennyPincher.Api.Extensions;

namespace PennyPincher.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class StatementsController : ErrorOrApiController
{
    private readonly IStatementsService _statementsService;

    public StatementsController(IStatementsService statementsService)
    {
        _statementsService = statementsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] StatementFilterRequest filters, [FromQuery] StatementSortingRequest sorting)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(Error.Forbidden());

        var result = await _statementsService.GetByUserAsync(userId, filters, sorting);

        return result.Match(
            statements => Ok(statements),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] StatementRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(Error.Forbidden());

        var result = await _statementsService.InsertAsync(request, userId);

        return result.Match(
            statement => Created(string.Empty, statement),
            errors => Problem(errors)
        );
    }

    [HttpPut("{statementId}")]
    public async Task<IActionResult> Put(int statementId, [FromBody] StatementRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(Error.Forbidden());

        var result = await _statementsService.UpdateAsync(userId, statementId, request);

        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(Error.Forbidden());

        var result = await _statementsService.DeleteAsync(userId, id);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPut("markAllUncheckedNow")]
    public async Task<IActionResult> MarkAllUncheckedNowAsync()
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(Error.Forbidden());

        var result = await _statementsService.MarkAllUncheckedNowAsync(userId);
        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

#if DEBUG
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _statementsService.GetAllAsync();

        return result.Match(
            statements => Ok(statements),
            errors => Problem(errors)
        );
    }
#endif
}
