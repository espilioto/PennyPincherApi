using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Data;
using PennyPincher.Domain.Models;

namespace PennyPincher.Tests.Helpers;

public static class TestDbContextFactory
{
    public static PennyPincherApiDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<PennyPincherApiDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new PennyPincherApiDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task SeedUserAsync(PennyPincherApiDbContext context, string userId, string email = "test@test.com")
    {
        context.Users.Add(new IdentityUser { Id = userId, UserName = email, Email = email, NormalizedEmail = email.ToUpper() });
        await context.SaveChangesAsync();
    }

    public static async Task SeedAccountAsync(PennyPincherApiDbContext context, int id, string userId, string name = "Test Account", string colorHex = "#FF0000", int sortOrder = 0)
    {
        context.Accounts.Add(new Account { Id = id, Name = name, UserId = userId, ColorHex = colorHex, SortOrder = sortOrder });
        await context.SaveChangesAsync();
    }

    public static async Task SeedCategoryAsync(PennyPincherApiDbContext context, int id, string userId, string name = "Test Category", int sortOrder = 0)
    {
        context.Categories.Add(new Category { Id = id, Name = name, UserId = userId, SortOrder = sortOrder });
        await context.SaveChangesAsync();
    }

    public static async Task SeedStatementAsync(PennyPincherApiDbContext context, string userId, int accountId, int categoryId, decimal amount, DateTime? date = null, string description = "Test statement")
    {
        context.Statements.Add(new Statement
        {
            Date = date ?? DateTime.UtcNow,
            AccountId = accountId,
            Amount = amount,
            Description = description,
            CategoryId = categoryId,
            UserId = userId
        });
        await context.SaveChangesAsync();
    }
}
