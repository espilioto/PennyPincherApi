using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;
using PennyPincher.Data;
using PennyPincher.Services.Mapping;

namespace PennyPincher.Services.Statements;

public class StatementsService : IStatementsService
{
    private readonly ILogger<StatementsService> _logger;
    private readonly PennyPincherApiDbContext _context;

    public StatementsService(PennyPincherApiDbContext context, ILogger<StatementsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<bool>> InsertAsync(StatementRequest request, string userId)
    {
        try
        {
            var statement = request.ToEntity();
            statement.UserId = userId;
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

    public async Task<ErrorOr<IEnumerable<StatementResponse>>> GetAllAsync()
    {
        try
        {
            var statements = await _context.Statements
                .AsNoTracking()
                .OrderByDescending(s => s.Date)
                .Select(s => new StatementResponse(
                    s.Id,
                    s.Date,
                    s.Amount,
                    s.Description,
                    s.CheckedAt,
                    new CategoryResponse(s.Category!.Id, s.Category.Name),
                    new AccountResponseLite(s.Account!.Id, s.Account.Name)
                ))
                .ToListAsync();

            return statements.Count != 0 ? statements : Error.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<IEnumerable<StatementResponse>>> GetByUserAsync(string userId, StatementFilterRequest? filters = null, StatementSortingRequest? sorting = null)
    {
        try
        {
            var statementsQuery = _context.Statements
                .AsQueryable()
                .AsNoTracking()
                .Where(s => s.UserId == userId);

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

            var sortBy = sorting?.SortBy?.ToLower() ?? "date";
            var sortDir = sorting?.Direction ?? "desc";

            statementsQuery = sortBy switch
            {
                "amount" => sortDir == "asc" ? statementsQuery.OrderBy(s => s.Amount) : statementsQuery.OrderByDescending(s => s.Amount),
                _ => sortDir == "asc" ? statementsQuery.OrderBy(s => s.Date) : statementsQuery.OrderByDescending(s => s.Date)
            };

            var statements = await statementsQuery
                .Select(s => new StatementResponse(
                    s.Id,
                    s.Date,
                    s.Amount,
                    s.Description,
                    s.CheckedAt,
                    new CategoryResponse(s.Category!.Id, s.Category.Name),
                    new AccountResponseLite(s.Account!.Id, s.Account.Name)
                ))
                .ToListAsync();

            return statements.Count != 0 ? statements : Error.NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> UpdateAsync(string userId, int statementId, StatementRequest request)
    {
        try
        {
            var statement = await _context.Statements.FirstOrDefaultAsync(x => x.Id == statementId && x.UserId == userId);
            if (statement is null)
                return Error.NotFound(description: "Statement not found");

            statement.Date = request.Date;
            statement.AccountId = request.AccountId;
            statement.Amount = request.Amount;
            statement.Description = request.Description;
            statement.CategoryId = request.CategoryId;
            var success = await _context.SaveChangesAsync();

            return success == 1 ? true : Error.Failure(description: "Error updating statement");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> DeleteAsync(string userId, int statementId)
    {
        try
        {
            var statementToRemove = await _context.Statements.FirstOrDefaultAsync(x => x.Id == statementId && x.UserId == userId);
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

    /// <summary>
    /// Marks all statements where CheckedAt is null with the given datetime.
    /// </summary>
    public async Task<ErrorOr<bool>> MarkAllUncheckedNowAsync(string userId)
    {
        try
        {
            var now = DateTime.UtcNow;

            var statementsToUpdate = await _context.Statements
                .Where(x => x.CheckedAt == null && x.UserId == userId)
                .ToListAsync();

            foreach (var statement in statementsToUpdate)
                statement.CheckedAt = now;

            var success = await _context.SaveChangesAsync();

            return success > 0 ? true : Error.Failure(description: "Error marking statements as checked");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
