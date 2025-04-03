using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Web.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2")]
public class StatementsController : ErrorOrApiController
{
    private readonly ILogger<StatementsController> _logger;
    private readonly IStatementsService _statementsService;
    private readonly IMapper _mapper;

    public StatementsController(ILogger<StatementsController> logger, IStatementsService statementsService, IMapper mapper)
    {
        _logger = logger;
        _statementsService = statementsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] StatementFilterRequest filters, [FromQuery] StatementSortingRequest sorting)
    {
        var result = await _statementsService.GetAllAsync(filters, sorting);

        return result.Match(
            statements => Ok(_mapper.Map<List<StatementResponse>>(statements)),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] StatementRequest request)
    {
        var result = await _statementsService.InsertAsync(_mapper.Map<StatementDto>(request));

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
