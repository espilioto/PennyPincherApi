using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements
{
    public interface IStatementsService
    {
        public Task<bool> Delete(int statementId);
        public Task<IEnumerable<LegacyStatementDto>> GetAllLegacyAsync();
        public Task<IEnumerable<StatementDto>> GetAllAsync();
        public Task<bool> InsertAsync(StatementDto statementRequest);
        public Task<bool> UpdateAsync(StatementDto statementRequest);
    }
}
