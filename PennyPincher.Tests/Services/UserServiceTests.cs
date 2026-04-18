using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Users;
using PennyPincher.Data;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Categories;
using PennyPincher.Services.Statements;
using PennyPincher.Services.Users;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

// InMemory provider does not support ExecuteDeleteAsync (used by the
// sibling DeleteAllByUserAsync calls) or real transactions, so the
// happy-path and rollback-on-failure tests live in the SQLite-based
// suite tracked in TODO.md. Only the pre-purge guard paths are covered
// here.
public class UserServiceTests
{
    private static (UserService service, PennyPincherApiDbContext context, UserManager<IdentityUser> userManager) Build()
    {
        var dbName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<PennyPincherApiDbContext>(o => o
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        services.AddIdentityCore<IdentityUser>(o =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 0;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequiredUniqueChars = 0;
        }).AddEntityFrameworkStores<PennyPincherApiDbContext>();

        var scope = services.BuildServiceProvider().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PennyPincherApiDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var stmtLogger = LoggerFactory.Create(b => { }).CreateLogger<StatementsService>();
        var catLogger = LoggerFactory.Create(b => { }).CreateLogger<CategoriesService>();
        var usrLogger = LoggerFactory.Create(b => { }).CreateLogger<UserService>();

        var statementsService = new StatementsService(context, stmtLogger);
        var accountService = new AccountService(context, stmtLogger);
        var categoriesService = new CategoriesService(context, catLogger);
        var configuration = new ConfigurationBuilder().Build();

        var userService = new UserService(userManager, statementsService, accountService, categoriesService, context, configuration, usrLogger);
        return (userService, context, userManager);
    }

    [Fact]
    public async Task DeleteAsync_WrongPassword_DoesNotPurgeData()
    {
        var (service, context, userManager) = Build();
        var user = new IdentityUser { UserName = "test@test.com", Email = "test@test.com" };
        await userManager.CreateAsync(user, "correct-password");
        await TestDbContextFactory.SeedAccountAsync(context, 1, user.Id);
        await TestDbContextFactory.SeedCategoryAsync(context, 1, user.Id);
        await TestDbContextFactory.SeedStatementAsync(context, user.Id, 1, 1, 100m);

        var result = await service.DeleteAsync(user.Id, new DeleteAccountRequest("wrong-password"));

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Validation, result.FirstError.Type);
        Assert.NotNull(await userManager.FindByIdAsync(user.Id));
        Assert.Equal(1, await context.Accounts.CountAsync());
        Assert.Equal(1, await context.Categories.CountAsync());
        Assert.Equal(1, await context.Statements.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_UnknownUser_ReturnsNotFound()
    {
        var (service, _, _) = Build();

        var result = await service.DeleteAsync("ghost-user-id", new DeleteAccountRequest("whatever"));

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
    }

    [Fact]
    public async Task ChangePasswordAsync_UnknownUser_ReturnsNotFound()
    {
        var (service, _, _) = Build();

        var result = await service.ChangePasswordAsync("ghost-user-id", new ChangePasswordRequest("any", "new"));

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
    }

    [Fact]
    public async Task ChangePasswordAsync_WrongCurrentPassword_ReturnsValidationError()
    {
        var (service, _, userManager) = Build();
        var user = new IdentityUser { UserName = "test@test.com", Email = "test@test.com" };
        await userManager.CreateAsync(user, "correct-password");

        var result = await service.ChangePasswordAsync(user.Id, new ChangePasswordRequest("wrong-current", "new-pwd"));

        Assert.True(result.IsError);
        Assert.Equal(ErrorType.Validation, result.FirstError.Type);
    }
}
