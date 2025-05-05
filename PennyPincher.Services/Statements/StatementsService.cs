using AutoMapper;
using AutoMapper.QueryableExtensions;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Statements;
using PennyPincher.Data;
using PennyPincher.Domain.Models;

namespace PennyPincher.Services.Statements;

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

    public async Task<ErrorOr<bool>> InsertAsync(StatementRequest request)
    {
        try
        {
            var statement = _mapper.Map<Statement>(request);
            _ = await _context.Statements.AddAsync(statement);
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error creating statement");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: $"{ex.Message} {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : string.Empty)}");
        }
    }

    public async Task<ErrorOr<IEnumerable<StatementResponse>>> GetAllAsync(StatementFilterRequest? filters = null, StatementSortingRequest? sorting = null)
    {
        try
        {
            var statementsQuery = _context.Statements
                .AsQueryable()
                .AsNoTracking();

            if (filters is not null)
            {
                if (filters.AccountIdsIncluded is not null && filters.AccountIdsIncluded.Count > 0)
                    statementsQuery = statementsQuery.Where(s => filters.AccountIdsIncluded.Contains(s.AccountId));

                if (filters.CategoryIdsIncluded is not null && filters.CategoryIdsIncluded.Count > 0)
                    statementsQuery = statementsQuery.Where(s => filters.CategoryIdsIncluded.Contains(s.CategoryId));

                if (filters.AccountIdsExcluded is not null && filters.AccountIdsExcluded.Count > 0)
                    statementsQuery = statementsQuery.Where(s => !filters.AccountIdsExcluded.Contains(s.AccountId));

                if (filters.CategoryIdsExcluded is not null && filters.CategoryIdsExcluded.Count > 0)
                    statementsQuery = statementsQuery.Where(s => !filters.CategoryIdsExcluded.Contains(s.CategoryId));

                if (filters.DateFrom.HasValue)
                    statementsQuery = statementsQuery.Where(s => s.Date >= filters.DateFrom.Value);

                if (filters.DateTo.HasValue)
                    statementsQuery = statementsQuery.Where(s => s.Date <= filters.DateTo.Value);

                if (filters.MinAmount.HasValue)
                    statementsQuery = statementsQuery.Where(s => Math.Abs(s.Amount) >= filters.MinAmount.Value);

                if (filters.MaxAmount.HasValue)
                    statementsQuery = statementsQuery.Where(s => Math.Abs(s.Amount) <= filters.MaxAmount.Value);
            }

            statementsQuery = sorting.SortBy.ToLower() switch
            {
                "amount" => sorting.Direction == "asc" ? statementsQuery.OrderBy(s => s.Amount) : statementsQuery.OrderByDescending(s => s.Amount),
                _ => sorting.Direction == "asc" ? statementsQuery.OrderBy(s => s.Date) : statementsQuery.OrderByDescending(s => s.Date)
            };

            var statements = await statementsQuery
                .ProjectTo<StatementResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return statements.Count != 0 ? statements : Error.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> UpdateAsync(int statementId, StatementRequest request)
    {
        try
        {
            var statement = await _context.Statements.FirstOrDefaultAsync(x => x.Id == statementId);
            if (statement is null)
                return Error.NotFound(description: "Statement not found");

            _mapper.Map(request, statement);
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error updating statement");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> DeleteAsync(int statementId)
    {
        try
        {
            var statementToRemove = await _context.Statements.FirstOrDefaultAsync(x => x.Id == statementId);
            if (statementToRemove is null)
                return Error.NotFound(description: "Statement not found");

            _context.Statements.Remove(statementToRemove);
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error deleting statement");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
