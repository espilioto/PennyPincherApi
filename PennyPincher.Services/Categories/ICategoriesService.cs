using ErrorOr;
using PennyPincher.Contracts.Categories;

namespace PennyPincher.Services.Categories
{
    public interface ICategoriesService
    {
        public Task<ErrorOr<bool>> DeleteAsync(int categoryId);
        public Task<ErrorOr<IEnumerable<CategoryResponse>>> GetAllAsync();
        public Task<ErrorOr<bool>> InsertAsync(CategoryRequest categoryRequest);
        public Task<ErrorOr<bool>> UpdateAsync(CategoryRequest categoryRequest);
    }
}
