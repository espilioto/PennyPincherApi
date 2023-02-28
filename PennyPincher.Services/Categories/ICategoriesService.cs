using PennyPincher.Services.Categories.Models;

namespace PennyPincher.Services.Categories
{
    public interface ICategoriesService
    {
        public Task<bool> Delete(int categoryId);
        public Task<IEnumerable<CategoryDto>> GetAllAsync();
        public Task<bool> InsertAsync(CategoryDto categoryRequest);
        public Task<bool> UpdateAsync(CategoryDto categoryRequest);
    }
}
