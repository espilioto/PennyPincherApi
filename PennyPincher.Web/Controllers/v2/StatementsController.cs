using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Accounts.Models;
using PennyPincher.Services.Statements;

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
    public async Task<IActionResult> Post(AccountDto accountRequest)
    {
        throw new NotImplementedException();
    }
}
