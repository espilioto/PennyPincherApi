using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements
{
    public class StatementsService : IStatementsService
    {
        private readonly PennyPincherApiDbContext _context;
        private readonly IMapper _mapper;

        public StatementsService(PennyPincherApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> InsertAsync(StatementDto statementRequest)
        {
            var statement = _mapper.Map<Statement>(statementRequest);
            _ = await _context.Statements.AddAsync(statement);
            var success = await _context.SaveChangesAsync();

            return success == 1;
        }

        public async Task<IEnumerable<StatementDto>> GetAllAsync()
        {
            var result = new List<StatementDto>();

            var statements = await _context.Statements.OrderByDescending(x => x.Id).ToListAsync();

            foreach (var item in statements)
            {
                result.Add(_mapper.Map<StatementDto>(item));
            }

            return result;
        }

        public async Task<bool> UpdateAsync(StatementDto statementRequest)
        {
            _ = _context.Update(statementRequest);
            var success = await _context.SaveChangesAsync();

            return success == 1;
        }

        public async Task<bool> Delete(int statementId)
        {
            var statementToRemove = await _context.Statements.FirstOrDefaultAsync(x => x.Id == statementId);

            if (statementToRemove is not null)
            {
                _context.Statements.Remove(statementToRemove);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
