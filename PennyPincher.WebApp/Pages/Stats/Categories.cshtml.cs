using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.WebApp.Pages.Stats;

[Authorize]
public class CategoriesModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CategoriesModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<CategoryResponse> Categories { get; set; } = [];
    public CategoryAnalyticsResponse? Analytics { get; set; }
    public int? SelectedCategoryId { get; set; }
    public string? SelectedCategoryName { get; set; }

    public async Task OnGetAsync(int? categoryId)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Categories = await client.GetFromJsonAsync<List<CategoryResponse>>("api/categories") ?? [];

        if (categoryId.HasValue)
        {
            SelectedCategoryId = categoryId;
            SelectedCategoryName = Categories.FirstOrDefault(c => c.Id == categoryId)?.Name;
            Analytics = await client.GetFromJsonAsync<CategoryAnalyticsResponse>(
                $"api/charts/GetCategoryAnalyticsChartData?categoryId={categoryId}");
        }
    }

    public async Task<IActionResult> OnGetAnalyticsAsync(int categoryId)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Categories = await client.GetFromJsonAsync<List<CategoryResponse>>("api/categories") ?? [];
        SelectedCategoryId = categoryId;
        SelectedCategoryName = Categories.FirstOrDefault(c => c.Id == categoryId)?.Name;
        Analytics = await client.GetFromJsonAsync<CategoryAnalyticsResponse>(
            $"api/charts/GetCategoryAnalyticsChartData?categoryId={categoryId}");
        return Partial("_CategoryAnalytics", this);
    }
}
