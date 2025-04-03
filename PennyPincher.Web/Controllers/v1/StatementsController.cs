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
        private readonly ILogger<StatementsController> _logger;
        private readonly IStatementsService _statementsService;

        public StatementsController(IStatementsService statementsService, ILogger<StatementsController> logger)
        {
            _statementsService = statementsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var statements = await _statementsService.GetAllLegacyAsync();
                return new JsonResult(statements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(LegacyStatementDto statementRequest)
        {
            try
            {
                var result = await _statementsService.InsertLegacyAsync(statementRequest);

                return result ? Created(string.Empty, result) : StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] LegacyStatementDto request)
        {
            try
            {
                var result = false;

                if (request is not null)
                    result = await _statementsService.UpdateLegacyAsync(request);

                return result ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Obsolete]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _statementsService.DeleteAsync(id);

                //return result ? Ok() : NotFound();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
