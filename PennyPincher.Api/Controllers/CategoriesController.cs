using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PennyPincher.Contracts.Categories;
using PennyPincher.Services.Categories;
using PennyPincher.Api.Extensions;

namespace PennyPincher.Api.Controllers;

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
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _categoriesService.GetByUserAsync(userId);

        return result.Match(
            categories => Ok(categories),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post(CategoryRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _categoriesService.InsertAsync(request, userId);

        return result.Match(
            category => Created(string.Empty, category),
            errors => Problem(errors)
        );
    }

    [HttpPut("{categoryId}")]
    public async Task<IActionResult> Put(int categoryId, [FromBody] CategoryRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _categoriesService.UpdateAsync(userId, categoryId, request);

        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] List<int> categoryIds)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _categoriesService.UpdateOrderAsync(userId, categoryIds);

        return result.Match(
            _ => Ok(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Problem(ErrorOr.Error.Forbidden());

        var result = await _categoriesService.DeleteAsync(userId, id);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}
