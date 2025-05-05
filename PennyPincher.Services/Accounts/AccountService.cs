using AutoMapper;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Data;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Statements;
using System.Text.RegularExpressions;

namespace PennyPincher.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly PennyPincherApiDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<StatementsService> _logger;

    public AccountService(PennyPincherApiDbContext context, IMapper mapper, ILogger<StatementsService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ErrorOr<List<AccountResponse>>> GetAllAsync()
    {
        try
        {
            var accounts = await _context.Accounts
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

            return accounts.Count == 0 ? Error.NotFound() : accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> InsertAsync(AccountRequest request)
    {
        List<Error> errors = [];

        try
        {
            if (!await _context.Users.AnyAsync(x => x.Id == request.UserId))
                errors.Add(Error.Validation(description: "User does not exist"));

            if (!Regex.IsMatch(request.ColorHex, @"[#][0-9A-Fa-f]{6}\b"))
                errors.Add(Error.Validation(description: "Color value incorrect"));

            if (errors.Count > 0)
                return errors;

            var account = _mapper.Map<Account>(request);
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

    public async Task<ErrorOr<bool>> UpdateAsync(int accountId, AccountRequest request)
    {
        List<Error> errors = [];

        try
        {
            var account = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == accountId);
            if (account is null)
                return Error.NotFound(description: "Account not found");

            if (!await _context.Users.AnyAsync(x => x.Id == request.UserId))
                errors.Add(Error.Validation(description: "User does not exist"));

            if (!Regex.IsMatch(request.ColorHex, @"[#][0-9A-Fa-f]{6}\b"))
                errors.Add(Error.Validation(description: "Color value incorrect"));

            if (errors.Count > 0)
                return errors;

            _ = _context.Accounts.Update(_mapper.Map<Account>(request));
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error updating account");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> DeleteAsync(int accountId)
    {
        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
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
}
