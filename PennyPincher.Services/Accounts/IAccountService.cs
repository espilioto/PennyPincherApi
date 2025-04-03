using ErrorOr;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Services.Accounts.Models;

namespace PennyPincher.Services.Accounts;

public interface IAccountService
{
    public Task<ErrorOr<bool>> DeleteAsync(int accountId);
    public Task<IEnumerable<AccountDto>> GetAllAsync();
    public Task<ErrorOr<List<AccountResponse>>> GetAllAsyncV2();
    public Task<bool> InsertAsync(AccountDto request);
    public Task<ErrorOr<bool>> InsertAsync(AccountRequest request);
    public Task<bool> UpdateAsync(AccountDto request);
    public Task<ErrorOr<bool>> UpdateAsync(AccountRequest request);
}
