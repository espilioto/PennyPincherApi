using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Categories;
using PennyPincher.Services.Categories;

namespace PennyPincher.Web.Controllers;

[Authorize]
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

    [HttpPut("{categoryId}")]
    public async Task<IActionResult> Put(int categoryId, [FromBody] CategoryRequest request)
    {
        var result = await _categoriesService.UpdateAsync(categoryId, request);

        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoriesService.DeleteAsync(id);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
