using PennyPincher.Services.Accounts.Models;

namespace PennyPincher.Services.Accounts
{
    public interface IAccountService
    {
        public Task<bool> Delete(int accountId);
        public Task<IEnumerable<AccountDto>> GetAllAsync();
        public Task<bool> InsertAsync(AccountDto accountRequest);
        public Task<bool> UpdateAsync(AccountDto accountRequest);
    }
}
