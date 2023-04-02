using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Services.Statements
{
    public class StatementsService : IStatementsService
    {
        private readonly ILogger<StatementsService> _logger;
        private readonly IMapper _mapper;
        private readonly PennyPincherApiDbContext _context;

        public StatementsService(PennyPincherApiDbContext context, IMapper mapper, ILogger<StatementsService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> InsertAsync(StatementDto statementRequest)
        {
            try
            {
                var statement = _mapper.Map<Statement>(statementRequest);
                _ = await _context.Statements.AddAsync(statement);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> InsertLegacyAsync(LegacyStatementDto statementRequest)
        {
            try
            {
                var statement = _mapper.Map<Statement>(statementRequest);
                _ = await _context.Statements.AddAsync(statement);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<LegacyStatementDto>> GetAllLegacyAsync()
        {
            try
            {
                var result = new List<LegacyStatementDto>();

                var statementsQuery = _context.Statements
                    .AsQueryable()
                    .Include(x => x.Category.Name)
                    .Include(x => x.Account.Name)
                    .AsNoTracking();

                var statements = await statementsQuery.OrderByDescending(x => x.Id).ToListAsync();

                foreach (var item in statements)
                {
                    result.Add(_mapper.Map<LegacyStatementDto>(item));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<LegacyStatementDto>();
            }
        }

        public async Task<IEnumerable<StatementDto>> GetAllAsync()
        {
            try
            {
                var result = new List<StatementDto>();

                var statements = await _context.Statements.OrderByDescending(x => x.Id).ToListAsync();

                foreach (var item in statements)
                {
                    result.Add(_mapper.Map<StatementDto>(item));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<StatementDto>();
            }
        }

        public async Task<bool> UpdateAsync(StatementDto statementRequest)
        {
            try
            {
                _ = _context.Update(statementRequest);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateLegacyAsync(LegacyStatementDto statementRequest)
        {
            try
            {
                var statement = _mapper.Map<Statement>(statementRequest);
                _ = _context.Update(statement);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int statementId)
        {
            try
            {
                var statementToRemove = await _context.Statements.FirstOrDefaultAsync(x => x.Id == statementId);

                if (statementToRemove is not null)
                {
                    _context.Statements.Remove(statementToRemove);
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
    }
}
