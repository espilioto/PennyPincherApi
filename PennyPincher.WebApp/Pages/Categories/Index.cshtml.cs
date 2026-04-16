using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Categories;

namespace PennyPincher.WebApp.Pages.Categories;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<CategoryResponse> Categories { get; set; } = [];

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Categories = await client.GetFromJsonAsync<List<CategoryResponse>>("api/categories") ?? [];
    }

    public async Task<IActionResult> OnPostCreateAsync(string name)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var request = new CategoryRequest(name, "");
        var response = await client.PostAsJsonAsync("api/categories", request);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostEditAsync(int id, string name)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var request = new CategoryRequest(name, "");
        var response = await client.PutAsJsonAsync($"api/categories/{id}", request);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var response = await client.DeleteAsync($"api/categories/{id}");

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostReorderAsync([FromBody] List<int> orderedIds)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var response = await client.PutAsJsonAsync("api/categories/reorder", orderedIds);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }
}
