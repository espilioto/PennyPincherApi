using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Categories;
using PennyPincher.Services.Categories;
using PennyPincher.Tests.Helpers;

namespace PennyPincher.Tests.Services;

public class CategoriesServiceTests
{
    private readonly ILogger<CategoriesService> _logger = LoggerFactory.Create(b => { }).CreateLogger<CategoriesService>();

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport");

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenEmpty()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        var result = await service.GetByUserAsync("user1");

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task InsertAsync_AcceptsValidRequest()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");

        var request = new CategoryRequest("Food", "user1");
        var result = await service.InsertAsync(request, "user1");

        Assert.False(result.IsError);
        Assert.Single(context.Categories);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesName()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Old Name");

        var request = new CategoryRequest("New Name", "user1");
        var result = await service.UpdateAsync("user1", 1, request);

        Assert.False(result.IsError);
        Assert.Equal("New Name", context.Categories.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenMissing()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        var request = new CategoryRequest("Test", "user1");
        var result = await service.UpdateAsync("user1", 999, request);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCategory()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");

        var result = await service.DeleteAsync("user1", 1);

        Assert.False(result.IsError);
        Assert.Empty(context.Categories);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenMissing()
    {
        var context = TestDbContextFactory.Create();
        var mapper = TestDbContextFactory.CreateMapper();
        var service = new CategoriesService(context, mapper, _logger);

        var result = await service.DeleteAsync("user1", 999);

        Assert.True(result.IsError);
    }
}
