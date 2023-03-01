using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Accounts.Models;

namespace PennyPincher.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly PennyPincherApiDbContext _context;
        private readonly IMapper _mapper;

        public AccountService(PennyPincherApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> InsertAsync(AccountDto accountRequest)
        {
            var account = _mapper.Map<Account>(accountRequest);
            _ = await _context.Accounts.AddAsync(account);
            var success = await _context.SaveChangesAsync();

            return success == 1;
        }

        public async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            var result = new List<AccountDto>();

            var accounts = await _context.Accounts.ToListAsync();

            foreach (var item in accounts)
            {
                result.Add(_mapper.Map<AccountDto>(item));
            }

            return result;
        }

        public async Task<bool> UpdateAsync(AccountDto accountRequest)
        {
            _ = _context.Update(accountRequest);
            var success = await _context.SaveChangesAsync();

            return success == 1;
        }

        public async Task<bool> Delete(int accountId)
        {
            var accountToRemove = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);

            if (accountToRemove is not null)
            {
                _context.Accounts.Remove(accountToRemove);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
