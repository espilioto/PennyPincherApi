using AutoMapper;
using Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Accounts.Models;
using PennyPincher.Services.Statements;

namespace PennyPincher.Services.Accounts
{
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

        public async Task<bool> InsertAsync(AccountDto accountRequest)
        {
            try
            {
                var account = _mapper.Map<Account>(accountRequest);
                _ = await _context.Accounts.AddAsync(account);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            try
            {
                var result = new List<AccountDto>();

                var accounts = await _context.Accounts.ToListAsync();

                foreach (var item in accounts)
                {
                    result.Add(_mapper.Map<AccountDto>(item));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<AccountDto>();
            }
        }

        public async Task<bool> UpdateAsync(AccountDto accountRequest)
        {
            try
            {
                _ = _context.Update(accountRequest);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int accountId)
        {
            try
            {
                var accountToRemove = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);

                if (accountToRemove is not null)
                {
                    _context.Accounts.Remove(accountToRemove);
                    var success = await _context.SaveChangesAsync();

                    return success == 1;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public Task<ErrorOr<List<AccountResponse>>> GetAllAsyncV2()
        {
            throw new NotImplementedException();
        }
    }
}
