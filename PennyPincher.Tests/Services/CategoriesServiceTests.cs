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
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport");

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenEmpty()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task InsertAsync_AcceptsValidRequest()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

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
        var service = new CategoriesService(context, _logger);

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
        var service = new CategoriesService(context, _logger);

        var request = new CategoryRequest("Test", "user1");
        var result = await service.UpdateAsync("user1", 999, request);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCategory()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

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
        var service = new CategoriesService(context, _logger);

        var result = await service.DeleteAsync("user1", 999);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsCategoriesOrderedBySortOrder()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Last", sortOrder: 2);
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "First", sortOrder: 0);
        await TestDbContextFactory.SeedCategoryAsync(context, 3, "user1", "Middle", sortOrder: 1);

        var result = await service.GetByUserAsync("user1");

        Assert.False(result.IsError);
        var categories = result.Value.ToList();
        Assert.Equal("First", categories[0].Name);
        Assert.Equal("Middle", categories[1].Name);
        Assert.Equal("Last", categories[2].Name);
    }

    [Fact]
    public async Task InsertAsync_SetsSortOrderToMaxPlusOne()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food", sortOrder: 0);
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport", sortOrder: 1);

        var request = new CategoryRequest("Rent", "user1");
        var result = await service.InsertAsync(request, "user1");

        Assert.False(result.IsError);
        var newCategory = context.Categories.OrderByDescending(c => c.SortOrder).First();
        Assert.Equal("Rent", newCategory.Name);
        Assert.Equal(2, newCategory.SortOrder);
    }

    [Fact]
    public async Task InsertAsync_SetsSortOrderToZero_WhenNoCategories()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");

        var request = new CategoryRequest("Food", "user1");
        var result = await service.InsertAsync(request, "user1");

        Assert.False(result.IsError);
        Assert.Equal(0, context.Categories.First().SortOrder);
    }

    [Fact]
    public async Task UpdateOrderAsync_ReordersCategoriesCorrectly()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food", sortOrder: 0);
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport", sortOrder: 1);
        await TestDbContextFactory.SeedCategoryAsync(context, 3, "user1", "Rent", sortOrder: 2);

        var result = await service.UpdateOrderAsync("user1", [3, 1, 2]);

        Assert.False(result.IsError);
        Assert.Equal(0, context.Categories.First(c => c.Id == 3).SortOrder);
        Assert.Equal(1, context.Categories.First(c => c.Id == 1).SortOrder);
        Assert.Equal(2, context.Categories.First(c => c.Id == 2).SortOrder);

        // Verify GET returns in new order
        var getResult = await service.GetByUserAsync("user1");
        var categories = getResult.Value.ToList();
        Assert.Equal("Rent", categories[0].Name);
        Assert.Equal("Food", categories[1].Name);
        Assert.Equal("Transport", categories[2].Name);
    }

    [Fact]
    public async Task UpdateOrderAsync_ReturnsError_WhenIdsDoNotMatchUserCategories()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport");

        var result = await service.UpdateOrderAsync("user1", [1, 999]);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task UpdateOrderAsync_ReturnsError_WhenCountMismatch()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food");
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport");

        var result = await service.UpdateOrderAsync("user1", [1]);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task UpdateOrderAsync_DoesNotAffectOtherUsers()
    {
        var context = TestDbContextFactory.Create();
        var service = new CategoriesService(context, _logger);

        await TestDbContextFactory.SeedUserAsync(context, "user1");
        await TestDbContextFactory.SeedUserAsync(context, "user2", "user2@test.com");
        await TestDbContextFactory.SeedCategoryAsync(context, 1, "user1", "Food", sortOrder: 0);
        await TestDbContextFactory.SeedCategoryAsync(context, 2, "user1", "Transport", sortOrder: 1);
        await TestDbContextFactory.SeedCategoryAsync(context, 3, "user2", "Rent", sortOrder: 0);

        var result = await service.UpdateOrderAsync("user1", [2, 1]);

        Assert.False(result.IsError);
        Assert.Equal(0, context.Categories.First(c => c.Id == 3).SortOrder);
    }
}
