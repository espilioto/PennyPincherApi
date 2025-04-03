using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Categories;
using PennyPincher.Services.Categories;

namespace PennyPincher.Web.Controllers.v2;

[ApiController]
[Route("api/[controller]")]

public class CategoriesController : ErrorOrApiController
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _categoriesService.GetAllAsync();

        return result.Match(
            categories => Ok(categories),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post(CategoryRequest request)
    {
        var result = await _categoriesService.InsertAsync(request);

        return result.Match(
            category => Created(string.Empty, category),
            errors => Problem(errors)
        );
    }
}
