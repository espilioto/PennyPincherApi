using ErrorOr;
using PennyPincher.Contracts.Accounts;

namespace PennyPincher.Services.Accounts;

public interface IAccountService
{
    public Task<ErrorOr<bool>> DeleteAsync(int accountId);
    public Task<ErrorOr<List<AccountResponse>>> GetAllAsync();
    public Task<ErrorOr<bool>> InsertAsync(AccountRequest request);
    public Task<ErrorOr<bool>> UpdateAsync(AccountRequest request);
}
