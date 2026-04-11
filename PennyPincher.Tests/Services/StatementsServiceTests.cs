using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Statements;
using PennyPincher.Services.Statements;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

public class StatementsServiceTests
{
    private readonly ILogger<StatementsService> _logger = LoggerFactory.Create(b => { }).CreateLogger<StatementsService>();

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyUserStatements()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedUserAsync(context, "user2", "other@test.com");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 100m, description: "Mine");
        await TestDbContextFactory.SeedStatementAsync(context, "user2", 1, 1, 200m, description: "Not mine");

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        var statements = result.Value.ToList();
        Assert.Single(statements);
        Assert.Equal("Mine", statements[0].Description);
    }

    [Fact]
    public async Task GetByUserAsync_WithNullSorting_DefaultsToDateDesc()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 100m, date: new DateTime(2024, 1, 1), description: "Older");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 200m, date: new DateTime(2024, 6, 1), description: "Newer");

        var result = await service.GetByUserAsync("user1", null, null);

        Assert.False(result.IsError);
        var statements = result.Value.ToList();
        Assert.Equal("Newer", statements[0].Description);
        Assert.Equal("Older", statements[1].Description);
    }

    [Fact]
    public async Task GetByUserAsync_SortByAmountAsc()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 500m, description: "Big");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 10m, description: "Small");

        var result = await service.GetByUserAsync("user1", null, new StatementSortingRequest("amount", "asc"));

        Assert.False(result.IsError);
        var statements = result.Value.ToList();
        Assert.Equal("Small", statements[0].Description);
        Assert.Equal("Big", statements[1].Description);
    }

    [Fact]
    public async Task GetByUserAsync_FilterByDateRange()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 100m, date: new DateTime(2024, 1, 15), description: "Jan");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 200m, date: new DateTime(2024, 3, 15), description: "Mar");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 300m, date: new DateTime(2024, 6, 15), description: "Jun");

        var filter = new StatementFilterRequest(null, null, null, null, new DateTime(2024, 2, 1), new DateTime(2024, 4, 1), null, null, null);
        var result = await service.GetByUserAsync("user1", filter);

        Assert.False(result.IsError);
        var statements = result.Value.ToList();
        Assert.Single(statements);
        Assert.Equal("Mar", statements[0].Description);
    }

    [Fact]
    public async Task GetByUserAsync_FilterByExcludedCategory()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Transfers");
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Food");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 100m, description: "Transfer");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 2, 50m, description: "Lunch");

        var filter = new StatementFilterRequest(null, null, null, [1], null, null, null, null, null);
        var result = await service.GetByUserAsync("user1", filter);

        Assert.False(result.IsError);
        var statements = result.Value.ToList();
        Assert.Single(statements);
        Assert.Equal("Lunch", statements[0].Description);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsNotFound_WhenNoStatements()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        var result = await service.GetByUserAsync("nonexistent");

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsersStatements()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedUserAsync(context, "user2", "other@test.com");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 100m);
        await TestDbContextFactory.SeedStatementAsync(context, "user2", 1, 1, 200m);

        var result = await service.GetAllAsync();

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task InsertAsync_SetsUserId()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");

        var request = new StatementRequest(DateTime.UtcNow, 1, 50m, "Test", 1);
        var result = await service.InsertAsync(request, "user1");

        Assert.False(result.IsError);
        var statement = context.Statements.First();
        Assert.Equal("user1", statement.UserId);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenMissing()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        var request = new StatementRequest(DateTime.UtcNow, 1, 50m, "Test", 1);
        var result = await service.UpdateAsync("user1", 999, request);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenMissing()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        var result = await service.DeleteAsync("user1", 999);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task MarkAllUncheckedNowAsync_OnlyUpdatesNullCheckedAt()
    {
        var context = TestDbContextFactory.Create();
        var service = new StatementsService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");

        var alreadyChecked = new DateTime(2024, 1, 1);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 100m, description: "Unchecked");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 200m, description: "Already checked");

        var checkedStatement = context.Statements.First(s => s.Description == "Already checked");
        checkedStatement.CheckedAt = alreadyChecked;
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await service.MarkAllUncheckedNowAsync("user1");

        var wasUnchecked = context.Statements.First(s => s.Description == "Unchecked");
        var stillChecked = context.Statements.First(s => s.Description == "Already checked");

        Assert.NotNull(wasUnchecked.CheckedAt);
        Assert.Equal(alreadyChecked, stillChecked.CheckedAt);
    }
}
