using Microsoft.AspNetCore.Mvc;
using PennyPincher.Services.Categories;
using PennyPincher.Services.Categories.Models;
using PennyPincher.Services.Statements.Models;

namespace PennyPincher.Web.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDto>> Get()
        {
            var categories = await _categoriesService.GetAllAsync();
            return categories;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoryDto categoryRequest)
        {
            var result = await _categoriesService.InsertAsync(categoryRequest);

            return result ? Created(string.Empty, result) : StatusCode(500);
        }
    }
}
