using ErrorOr;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements;

public interface IStatementsService
{
    public Task<ErrorOr<bool>> DeleteAsync(int statementId);
    public Task<ErrorOr<IEnumerable<StatementDtoV2>>> GetAllAsync(StatementFilterRequest filters, StatementSortingRequest sorting);
    public Task<ErrorOr<bool>> InsertAsync(StatementDto statementRequest);
    public Task<ErrorOr<bool>> UpdateAsync(StatementRequest statementRequest);
}
