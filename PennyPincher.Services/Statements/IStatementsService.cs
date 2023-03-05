using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements
{
    public interface IStatementsService
    {
        public Task<bool> Delete(int statementId);
        public Task<IEnumerable<LegacyStatementDto>> GetAllLegacyAsync();
        public Task<IEnumerable<StatementDto>> GetAllAsync();
        public Task<bool> InsertAsync(StatementDto statementRequest);
        Task<bool> InsertLegacyAsync(LegacyStatementDto statementRequest);
        public Task<bool> UpdateAsync(StatementDto statementRequest);
    }
}
