using AutoMapper;
using Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Services.Accounts.Models;
using PennyPincher.Services.Statements;

namespace PennyPincher.Services.Accounts;

public class AccountServiceV2 : IAccountService
{
    private readonly PennyPincherApiDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<StatementsService> _logger;

    public AccountServiceV2(PennyPincherApiDbContext context, IMapper mapper, ILogger<StatementsService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<bool> Delete(int accountId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<List<AccountResponse>>> GetAllAsyncV2()
    {
        try
        {
            var accounts = await _context.Accounts
                .Select(x => new AccountResponse
                (
                    x.Id,
                    x.Name,
                    _context.Statements
                        .Where(x => x.AccountId == x.Id)
                        .Sum(x => x.Amount)
                ))
                .ToListAsync();

            return accounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public Task<bool> InsertAsync(AccountDto accountRequest)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(AccountDto accountRequest)
    {
        throw new NotImplementedException();
    }
}
