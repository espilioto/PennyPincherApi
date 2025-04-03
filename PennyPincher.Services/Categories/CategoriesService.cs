using AutoMapper;
using Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Contracts.Categories;
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
                if (!await _context.Users.AnyAsync(x => x.Id == request.userId))
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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return Error.Unexpected(description: ex.Message);
            }
        }

        public async Task<ErrorOr<bool>> UpdateAsync(CategoryRequest categoryRequest)
        {
            //TODO
            throw new NotImplementedException();
        }

        public async Task<ErrorOr<bool>> Delete(int categoryId)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
