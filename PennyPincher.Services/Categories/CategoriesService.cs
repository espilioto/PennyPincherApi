using AutoMapper;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Categories;
using PennyPincher.Data;
using PennyPincher.Domain.Models;

namespace PennyPincher.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ILogger<CategoriesService> _logger;
        private readonly IMapper _mapper;
        private readonly PennyPincherApiDbContext _context;

        public CategoriesService(PennyPincherApiDbContext context, IMapper mapper, ILogger<CategoriesService> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ErrorOr<bool>> InsertAsync(CategoryRequest request)
        {
            List<Error> errors = [];

            try
            {
                if (!await _context.Users.AnyAsync(x => x.Id == request.UserId))
                    errors.Add(Error.Validation(description: "User does not exist"));

                if (errors.Count > 0)
                    return errors;

                var category = _mapper.Map<Category>(request);
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

        public async Task<ErrorOr<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            try
            {
                var categories = await _context.Categories
                .Select(x => new CategoryResponse
                (
                    x.Id,
                    x.Name
                ))
                .ToListAsync();

                return categories.Count == 0 ? Error.NotFound() : categories;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> UpdateAsync(int categoryId, CategoryRequest request)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
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

        public async Task<ErrorOr<bool>> DeleteAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);
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
    }
}
