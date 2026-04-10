using ErrorOr;
using PennyPincher.Contracts.Statements;

namespace PennyPincher.Services.Statements;

public interface IStatementsService
{
    public Task<ErrorOr<bool>> DeleteAsync(string userId, int statementId);
    public Task<ErrorOr<IEnumerable<StatementResponse>>> GetAllAsync();
    public Task<ErrorOr<IEnumerable<StatementResponse>>> GetByUserAsync(string userId, StatementFilterRequest? filters, StatementSortingRequest? sorting);
    public Task<ErrorOr<bool>> InsertAsync(StatementRequest statementRequest, string userId);
    public Task<ErrorOr<bool>> UpdateAsync(string userId, int statementId, StatementRequest statementRequest);
    public Task<ErrorOr<bool>> MarkAllUncheckedNowAsync(string userId);
}
