using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Statements;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Statements.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                    .Include(x => x.Category)
                    .Include(x => x.Account)
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

        public async Task<ErrorOr<IEnumerable<StatementDtoV2>>> GetAllAsync(StatementFilterRequest filters, StatementSortingRequest sorting)
        {
            try
            {
                var statementsQuery = _context.Statements
                    .AsQueryable()
                    .AsNoTracking();

                if (filters.AccountIds != null && filters.AccountIds.Any())
                {
                    statementsQuery = statementsQuery.Where(s => filters.AccountIds.Contains(s.AccountId));
                }

                if (filters.CategoryIds is not null && filters.CategoryIds.Any())
                {
                    statementsQuery = statementsQuery.Where(s => filters.CategoryIds.Contains(s.CategoryId));
                }

                if (filters.DateFrom.HasValue)
                {
                    statementsQuery = statementsQuery.Where(s => s.Date >= filters.DateFrom.Value);
                }

                if (filters.DateTo.HasValue)
                {
                    statementsQuery = statementsQuery.Where(s => s.Date <= filters.DateTo.Value);
                }

                if (filters.MinAmount.HasValue)
                {
                    statementsQuery = statementsQuery.Where(s => s.Amount >= filters.MinAmount.Value);
                }

                if (filters.MaxAmount.HasValue)
                {
                    statementsQuery = statementsQuery.Where(s => s.Amount <= filters.MaxAmount.Value);
                }

                statementsQuery = sorting.SortBy.ToLower() switch
                {
                    "amount" => sorting.Direction == "asc" ? statementsQuery.OrderBy(s => s.Amount) : statementsQuery.OrderByDescending(s => s.Amount),
                    _ => sorting.Direction == "asc" ? statementsQuery.OrderBy(s => s.Date) : statementsQuery.OrderByDescending(s => s.Date)
                };

                var statements = await statementsQuery
                    .ProjectTo<StatementDtoV2>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return statements.Any() ? statements : Error.NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Error.Unexpected(description: ex.Message);
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
