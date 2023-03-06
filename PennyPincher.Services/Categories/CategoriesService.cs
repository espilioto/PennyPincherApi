using AutoMapper;
using Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Categories.Models;

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

        public async Task<bool> InsertAsync(CategoryDto categoryRequest)
        {
            try
            {
                var category = _mapper.Map<Category>(categoryRequest);
                _ = await _context.Categories.AddAsync(category);
                var success = await _context.SaveChangesAsync();
                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            try
            {
                var result = new List<CategoryDto>();

                var categories = await _context.Categories.ToListAsync();

                foreach (var item in categories)
                {
                    result.Add(_mapper.Map<CategoryDto>(item));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Enumerable.Empty<CategoryDto>();
            }
        }

        public async Task<bool> UpdateAsync(CategoryDto categoryRequest)
        {
            try
            {
                _ = _context.Update(categoryRequest);
                var success = await _context.SaveChangesAsync();

                return success == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> Delete(int categoryId)
        {
            try
            {
                var categoryToRemove = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

                if (categoryToRemove is not null)
                {
                    _context.Categories.Remove(categoryToRemove);
                    var success = await _context.SaveChangesAsync();

                    return success == 1;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
