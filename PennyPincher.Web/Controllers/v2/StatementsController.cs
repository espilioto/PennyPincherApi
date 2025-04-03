using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements;

namespace PennyPincher.Web.Controllers.v2;

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
        var result = await _statementsService.GetAllAsync(filters, sorting);

        return result.Match(
            statements => Ok(statements),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] StatementRequest request)
    {
        var result = await _statementsService.InsertAsync(request);

        return result.Match(
            statement => Created(string.Empty, statement),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put([FromBody] StatementRequest request)
    {
        var result = await _statementsService.UpdateAsync(request);

        return result.Match(
          _ => Ok(),
          errors => Problem(errors)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _statementsService.DeleteAsync(id);

        return result.Match(
         _ => NoContent(),
         errors => Problem(errors)
     );
    }
}
