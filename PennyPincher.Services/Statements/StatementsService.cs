﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Statements;
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

    public async Task<ErrorOr<IEnumerable<StatementResponse>>> GetAllAsync(StatementFilterRequest filters, StatementSortingRequest sorting)
    {
        try
        {
            var statementsQuery = _context.Statements
                .AsQueryable()
                .AsNoTracking();

            if (filters.AccountIds != null && filters.AccountIds.Count != 0)
            {
                statementsQuery = statementsQuery.Where(s => filters.AccountIds.Contains(s.AccountId));
            }

            if (filters.CategoryIds is not null && filters.CategoryIds.Count != 0)
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
                statementsQuery = statementsQuery.Where(s => Math.Abs(s.Amount) >= filters.MinAmount.Value);
            }

            if (filters.MaxAmount.HasValue)
            {
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
        List<Error> errors = [];

        try
        {
            var statement = await _context.Statements.AsNoTracking().FirstOrDefaultAsync(x => x.Id == statementId);
            if (statement is null)
                return Error.NotFound(description: "Statement not found");

            if (errors.Count > 0)
                return errors;

            _ = _context.Statements.Update(_mapper.Map<Statement>(statement));
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
