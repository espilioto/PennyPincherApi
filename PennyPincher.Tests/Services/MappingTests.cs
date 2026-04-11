using Microsoft.EntityFrameworkCore;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Mapping;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

public class MappingTests
{
    [Fact]
    public void StatementRequest_ToEntity_MapsAllProperties()
    {
        var request = new StatementRequest(new DateTime(2024, 6, 15), 3, 99.50m, "Test description", 7);

        var entity = request.ToEntity();

        Assert.Equal(request.Date, entity.Date);
        Assert.Equal(request.AccountId, entity.AccountId);
        Assert.Equal(request.Amount, entity.Amount);
        Assert.Equal(request.Description, entity.Description);
        Assert.Equal(request.CategoryId, entity.CategoryId);
        Assert.Empty(entity.UserId);
        Assert.Null(entity.CheckedAt);
    }

    [Fact]
    public async Task Statement_SelectProjection_MapsAllProperties()
    {
        var context = TestDbContextFactory.Create();
        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1", "Savings", "#00FF00");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 123.45m, new DateTime(2024, 3, 10), "Groceries");

        var result = await context.Statements
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
            .ToListAsync(TestContext.Current.CancellationToken);

        Assert.Single(result);
        var r = result[0];
        Assert.Equal(123.45m, r.Amount);
        Assert.Equal("Groceries", r.Description);
        Assert.Equal("Food", r.Category.Name);
        Assert.Equal("Savings", r.Account.Name);
    }

    [Fact]
    public void CategoryRequest_ToEntity_MapsAllProperties()
    {
        var request = new CategoryRequest("Groceries", "user1");

        var entity = request.ToEntity();

        Assert.Equal(request.Name, entity.Name);
    }

    [Fact]
    public void AccountRequest_ToEntity_MapsAllProperties()
    {
        var request = new AccountRequest("Savings", "user1", "#FF5500");

        var entity = request.ToEntity();

        Assert.Equal(request.Name, entity.Name);
        Assert.Equal(request.ColorHex, entity.ColorHex);
    }
}
