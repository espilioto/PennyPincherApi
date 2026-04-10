using ErrorOr;
using PennyPincher.Contracts.Categories;

namespace PennyPincher.Services.Categories
{
    public interface ICategoriesService
    {
        public Task<ErrorOr<bool>> DeleteAsync(string userId, int categoryId);
        public Task<ErrorOr<IEnumerable<CategoryResponse>>> GetByUserAsync(string userId);
        public Task<ErrorOr<bool>> InsertAsync(CategoryRequest categoryRequest, string userId);
        public Task<ErrorOr<bool>> UpdateAsync(string userId, int categoryId, CategoryRequest categoryRequest);
    }
}
