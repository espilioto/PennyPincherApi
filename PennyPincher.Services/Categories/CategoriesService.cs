using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Categories.Models;

namespace PennyPincher.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly PennyPincherApiDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesService(PennyPincherApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> InsertAsync(CategoryDto categoryRequest)
        {
            var category = _mapper.Map<Category>(categoryRequest);
            _ = await _context.Categories.AddAsync(category);
            var success = await _context.SaveChangesAsync();

            return success == 1;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var result = new List<CategoryDto>();

            var categories = await _context.Categories.ToListAsync();

            foreach (var item in categories)
            {
                result.Add(_mapper.Map<CategoryDto>(item));
            }

            return result;
        }

        public async Task<bool> UpdateAsync(CategoryDto categoryRequest)
        {
            _ = _context.Update(categoryRequest);
            var success = await _context.SaveChangesAsync();

            return success == 1;
        }

        public async Task<bool> Delete(int categoryId)
        {
            var categoryToRemove = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (categoryToRemove is not null)
            {
                _context.Categories.Remove(categoryToRemove);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
