using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements
{
    public interface IStatementsService
    {
        public Task<bool> Delete(int statementId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeNavProperties">Used only by legacy api</param>
        /// <returns></returns>
        public Task<IEnumerable<StatementDto>> GetAllAsync(bool includeNavProperties = false);
        public Task<bool> InsertAsync(StatementDto statementRequest);
        public Task<bool> UpdateAsync(StatementDto statementRequest);
    }
}
