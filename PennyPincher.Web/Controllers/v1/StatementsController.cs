using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Statements;
using PennyPincher.Services.Statements.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PennyPincher.Web.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class StatementsController : ControllerBase
    {
        private readonly IStatementsService _statementsService;
        private readonly IMapper _mapper;

        public StatementsController(IStatementsService statementsService, IMapper mapper)
        {
            _statementsService = statementsService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<LegacyStatementDto>> GetAll()
        {
            var statements = await _statementsService.GetAllLegacyAsync();
            return statements;
        }

        [HttpPost]
        public async Task<IActionResult> Post(LegacyStatementDto statementRequest)
        {
            var result = await _statementsService.InsertLegacyAsync(statementRequest);

            return result ? Created(string.Empty, result) : StatusCode(500);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] LegacyStatementDto request)
        {
            var result = false;

            if (request is not null)
                result = await _statementsService.UpdateLegacyAsync(request);

            return result ? Ok() : StatusCode(500);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _statementsService.DeleteAsync(id);

            return result ? Ok() : StatusCode(500);
        }
    }
}
