using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;
using PennyPincher.Domain.Models;
using PennyPincher.Services;
using PennyPincher.Services.Mapping;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

public class MappingTests
{
    private readonly IMapper _mapper = TestDbContextFactory.CreateMapper();

    // --- StatementRequest → Statement ---

    [Fact]
    public void StatementRequest_ToEntity_MatchesAutoMapper()
    {
        var request = new StatementRequest(new DateTime(2024, 6, 15), 3, 99.50m, "Test description", 7);

        var autoMapped = _mapper.Map<Statement>(request);
        var manual = request.ToEntity();

        Assert.Equal(autoMapped.Date, manual.Date);
        Assert.Equal(autoMapped.AccountId, manual.AccountId);
        Assert.Equal(autoMapped.Amount, manual.Amount);
        Assert.Equal(autoMapped.Description, manual.Description);
        Assert.Equal(autoMapped.CategoryId, manual.CategoryId);
        Assert.Equal(autoMapped.UserId, manual.UserId); // both should be null/default
        Assert.Equal(autoMapped.CheckedAt, manual.CheckedAt); // both null
    }

    // --- SelectAsResponse (EF projection) ---

    [Fact]
    public async Task Statement_SelectAsResponse_MatchesProjectTo()
    {
        var context = TestDbContextFactory.Create();
        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1", "Savings", "#00FF00");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 123.45m, new DateTime(2024, 3, 10), "Groceries");

        var projected = await context.Statements
            .AsNoTracking()
            .ProjectTo<StatementResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var manual = await context.Statements
            .AsNoTracking()
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

        Assert.Single(projected);
        Assert.Single(manual);

        var p = projected[0];
        var m = manual[0];

        Assert.Equal(p.Id, m.Id);
        Assert.Equal(p.Date, m.Date);
        Assert.Equal(p.Amount, m.Amount);
        Assert.Equal(p.Description, m.Description);
        Assert.Equal(p.CheckedAt, m.CheckedAt);
        Assert.Equal(p.Category.Id, m.Category.Id);
        Assert.Equal(p.Category.Name, m.Category.Name);
        Assert.Equal(p.Account.Id, m.Account.Id);
        Assert.Equal(p.Account.Name, m.Account.Name);
    }

    // --- CategoryRequest → Category ---

    [Fact]
    public void CategoryRequest_ToEntity_MatchesAutoMapper()
    {
        var request = new CategoryRequest("Groceries", "user1");

        var autoMapped = _mapper.Map<Category>(request);
        var manual = request.ToEntity();

        Assert.Equal(autoMapped.Name, manual.Name);
    }

    // --- AccountRequest → Account ---

    [Fact]
    public void AccountRequest_ToEntity_MatchesAutoMapper()
    {
        var request = new AccountRequest("Savings", "user1", "#FF5500");

        var autoMapped = _mapper.Map<Account>(request);
        var manual = request.ToEntity();

        Assert.Equal(autoMapped.Name, manual.Name);
        Assert.Equal(autoMapped.ColorHex, manual.ColorHex);
    }
}
