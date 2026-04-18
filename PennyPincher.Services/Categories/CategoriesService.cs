using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Categories;
using PennyPincher.Data;
using PennyPincher.Services.Mapping;

namespace PennyPincher.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ILogger<CategoriesService> _logger;
        private readonly PennyPincherApiDbContext _context;

        public CategoriesService(PennyPincherApiDbContext context, ILogger<CategoriesService> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ErrorOr<bool>> InsertAsync(CategoryRequest request, string userId)
        {
            try
            {
                var maxOrder = await _context.Categories
                    .Where(x => x.UserId == userId)
                    .MaxAsync(x => (int?)x.SortOrder) ?? -1;

                var category = request.ToEntity();
                category.UserId = userId;
                category.SortOrder = maxOrder + 1;
                _ = await _context.Categories.AddAsync(category);
                var success = await _context.SaveChangesAsync();

                return success == 1 ? true : Error.Failure(description: "Error creating category");
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: $"{ex.Message} {(!string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : string.Empty)}");
            }
        }

        public async Task<ErrorOr<IEnumerable<CategoryResponse>>> GetByUserAsync(string userId)
        {
            try
            {
                var categories = await _context.Categories
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.SortOrder)
                .Select(x => new CategoryResponse
                (
                    x.Id,
                    x.Name,
                    x.SortOrder
                ))
                .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> UpdateAsync(string userId, int categoryId, CategoryRequest request)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId && x.UserId == userId);
                if (category is null)
                    return Error.NotFound(description: "Category not found");

                category.Name = request.Name;
                var success = await _context.SaveChangesAsync();

                return success == 1 ? true : Error.Failure(description: "Error updating category");
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> DeleteAsync(string userId, int categoryId)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId && x.UserId == userId);
                if (category is null)
                    return Error.NotFound(description: "Category not found");

                _context.Categories.Remove(category);
                var success = await _context.SaveChangesAsync();

                return success == 1 ? true : Error.Failure(description: "Error deleting category");
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> UpdateOrderAsync(string userId, List<int> categoryIds)
        {
            try
            {
                var categories = await _context.Categories
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (categoryIds.Count != categories.Count || !categoryIds.All(id => categories.Any(c => c.Id == id)))
                    return Error.Validation(description: "Invalid category IDs");

                for (var i = 0; i < categoryIds.Count; i++)
                {
                    var category = categories.First(c => c.Id == categoryIds[i]);
                    category.SortOrder = i;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> DeleteAllByUserAsync(string userId)
        {
            try
            {
                // ExecuteDeleteAsync bypasses the EF change tracker; relies on the DB-level cascade from IdentityUser.
                await _context.Categories.Where(x => x.UserId == userId).ExecuteDeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }
    }
}
