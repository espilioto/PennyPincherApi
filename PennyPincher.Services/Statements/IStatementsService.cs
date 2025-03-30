using ErrorOr;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements;

public interface IStatementsService
{
    public Task<bool> DeleteAsync(int statementId);
    public Task<IEnumerable<LegacyStatementDto>> GetAllLegacyAsync();
    public Task<ErrorOr<IEnumerable<StatementDtoV2>>> GetAllAsync(StatementFilterRequest filters, StatementSortingRequest sorting);
    public Task<bool> InsertAsync(StatementDto statementRequest);
    Task<bool> InsertLegacyAsync(LegacyStatementDto statementRequest);
    public Task<bool> UpdateAsync(StatementDto statementRequest);
    public Task<bool> UpdateLegacyAsync(LegacyStatementDto statementRequest);
}
