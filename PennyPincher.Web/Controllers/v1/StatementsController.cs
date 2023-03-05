using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Statements;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Web.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class StatementsController : ControllerBase
    {
        private readonly IStatementsService _statementsService;

        public StatementsController(IStatementsService statementsService)
        {
            _statementsService = statementsService;
        }

        [HttpGet]
        public async Task<IEnumerable<LegacyStatementDto>> GetAll()
        {
            var statements = await _statementsService.GetAllLegacyAsync();
            return statements;
        }

        [HttpPost]
        public async Task<IActionResult> Post(StatementDto statementRequest)
        {
            var result = await _statementsService.InsertAsync(statementRequest);

            return result ? Created(string.Empty, result) : StatusCode(500);
        }
    }
}
