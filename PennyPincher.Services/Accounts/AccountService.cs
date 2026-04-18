using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Data;
using PennyPincher.Services.Mapping;
using PennyPincher.Services.Statements;
using System.Text.RegularExpressions;

namespace PennyPincher.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly PennyPincherApiDbContext _context;
    private readonly ILogger<StatementsService> _logger;

    public AccountService(PennyPincherApiDbContext context, ILogger<StatementsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<List<AccountResponse>>> GetAllAsync()
    {
        try
        {
            var accounts = await _context.Accounts
                .OrderBy(a => a.SortOrder)
                .Select(a => new AccountResponse
                (
                    a.Id,
                    a.Name,
                    _context.Statements
                        .Where(x => x.AccountId == a.Id)
                        .Sum(x => x.Amount),
                    a.ColorHex
                ))
                .ToListAsync();

            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<List<AccountResponse>>> GetByUserAsync(string userId)
    {
        try
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.SortOrder)
                .Select(a => new AccountResponse
                (
                    a.Id,
                    a.Name,
                    _context.Statements
                        .Where(x => x.AccountId == a.Id)
                        .Sum(x => x.Amount),
                    a.ColorHex
                ))
                .ToListAsync();

            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> InsertAsync(AccountRequest request, string userId)
    {
        List<Error> errors = [];

        try
        {
            if (!Regex.IsMatch(request.ColorHex, @"[#][0-9A-Fa-f]{6}\b"))
                errors.Add(Error.Validation(description: "Color value incorrect"));

            if (errors.Count > 0)
                return errors;

            var maxOrder = await _context.Accounts
                .Where(a => a.UserId == userId)
                .MaxAsync(a => (int?)a.SortOrder) ?? -1;

            var account = request.ToEntity();
            account.UserId = userId;
            account.SortOrder = maxOrder + 1;
            _ = await _context.Accounts.AddAsync(account);
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error creating account");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: $"{ex.Message} {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : string.Empty)}");
        }
    }

    public async Task<ErrorOr<bool>> UpdateAsync(string userId, int accountId, AccountRequest request)
    {
        List<Error> errors = [];

        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId && x.UserId == userId);
            if (account is null)
                return Error.NotFound(description: "Account not found");

            if (!Regex.IsMatch(request.ColorHex, @"[#][0-9A-Fa-f]{6}\b"))
                errors.Add(Error.Validation(description: "Color value incorrect"));

            if (errors.Count > 0)
                return errors;

            account.Name = request.Name;
            account.ColorHex = request.ColorHex;
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error updating account");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> DeleteAsync(string userId, int accountId)
    {
        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId && x.UserId == userId);
            if (account is null)
                return Error.NotFound(description: "Account not found");

            _context.Accounts.Remove(account);
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error deleting account");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> UpdateOrderAsync(string userId, List<int> accountIds)
    {
        try
        {
            var accounts = await _context.Accounts
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (accountIds.Count != accounts.Count || !accountIds.All(id => accounts.Any(a => a.Id == id)))
                return Error.Validation(description: "Invalid account IDs");

            for (var i = 0; i < accountIds.Count; i++)
            {
                var account = accounts.First(a => a.Id == accountIds[i]);
                account.SortOrder = i;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> DeleteAllByUserAsync(string userId)
    {
        try
        {
            // ExecuteDeleteAsync bypasses the EF change tracker; relies on the DB-level cascade from IdentityUser.
            await _context.Accounts.Where(x => x.UserId == userId).ExecuteDeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
