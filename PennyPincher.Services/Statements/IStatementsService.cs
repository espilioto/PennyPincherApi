using ErrorOr;
using PennyPincher.Contracts.Statements;

namespace PennyPincher.Services.Statements;

public interface IStatementsService
{
    public Task<ErrorOr<bool>> DeleteAsync(int statementId);
    public Task<ErrorOr<IEnumerable<StatementResponse>>> GetAllAsync(StatementFilterRequest filters, StatementSortingRequest sorting);
    public Task<ErrorOr<bool>> InsertAsync(StatementRequest statementRequest);
    public Task<ErrorOr<bool>> UpdateAsync(int statementId, StatementRequest statementRequest);
}
