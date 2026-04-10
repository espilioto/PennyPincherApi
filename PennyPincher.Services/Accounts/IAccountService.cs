using ErrorOr;
using PennyPincher.Contracts.Accounts;

namespace PennyPincher.Services.Accounts;

public interface IAccountService
{
    public Task<ErrorOr<bool>> DeleteAsync(string userId, int accountId);
    public Task<ErrorOr<List<AccountResponse>>> GetAllAsync();
    public Task<ErrorOr<List<AccountResponse>>> GetByUserAsync(string userId);
    public Task<ErrorOr<bool>> InsertAsync(AccountRequest request, string userId);
    public Task<ErrorOr<bool>> UpdateAsync(string userId, int accountId, AccountRequest request);
}
