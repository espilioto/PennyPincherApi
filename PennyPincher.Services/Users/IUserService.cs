using ErrorOr;
using PennyPincher.Contracts.Users;

namespace PennyPincher.Services.Users;

public interface IUserService
{
    public Task<ErrorOr<List<UserResponse>>> GetAllAsync();
    public Task<ErrorOr<bool>> RegisterAsync(RegisterRequest request);
    public Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request);
    public Task<ErrorOr<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    public Task<ErrorOr<bool>> DeleteAsync(string userId, DeleteAccountRequest request);
}
