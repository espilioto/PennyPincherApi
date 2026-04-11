using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Statements;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

public class AccountServiceTests
{
    private readonly ILogger<StatementsService> _logger = LoggerFactory.Create(b => { }).CreateLogger<StatementsService>();

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyUserAccounts()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedUserAsync(context, "user2", "other@test.com");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1", "My Account");
        await TestDbContextFactory.SeedAccountAsync(context, 2, "user2", "Not My Account");

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        Assert.Single(result.Value);
        Assert.Equal("My Account", result.Value[0].Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsersAccounts()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedUserAsync(context, "user2", "other@test.com");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1", "Account 1");
        await TestDbContextFactory.SeedAccountAsync(context, 2, "user2", "Account 2");

        var result = await service.GetAllAsync();

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetByUserAsync_CalculatesBalanceFromStatements()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1");
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, 1000m);
        await TestDbContextFactory.SeedStatementAsync(context, "user1", 1, 1, -250m);

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        Assert.Equal(750m, result.Value[0].Balance);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsNotFound_WhenNoAccounts()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        var result = await service.GetByUserAsync("nonexistent");

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task InsertAsync_RejectsInvalidColorHex()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");

        var request = new AccountRequest("Bad Color", "user1", "not-a-color");
        var result = await service.InsertAsync(request, "user1");

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task InsertAsync_AcceptsValidRequest()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");

        var request = new AccountRequest("Savings", "user1", "#00FF00");
        var result = await service.InsertAsync(request, "user1");

        Assert.False(result.IsError);
        Assert.Single(context.Accounts);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenMissing()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        var request = new AccountRequest("Test", "user1", "#FF0000");
        var result = await service.UpdateAsync("user1", 999, request);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenMissing()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        var result = await service.DeleteAsync("user1", 999);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsAccountsOrderedBySortOrder()
    {
        var context = TestDbContextFactory.Create();
        var service = new AccountService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedAccountAsync(context, 1, "user1", "Last", sortOrder: 2);
        await TestDbContextFactory.SeedAccountAsync(context, 2, "user1", "First", sortOrder: 0);
        await TestDbContextFactory.SeedAccountAsync(context, 3, "user1", "Middle", sortOrder: 1);

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        Assert.Equal("First", result.Value[0].Name);
        Assert.Equal("Middle", result.Value[1].Name);
        Assert.Equal("Last", result.Value[2].Name);
    }
}
