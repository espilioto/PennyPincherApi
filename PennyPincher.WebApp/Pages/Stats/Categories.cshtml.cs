using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Charts;
using PennyPincher.Contracts.Statements;

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
    public List<StatementResponse> YearStatements { get; set; } = [];
    public int? SelectedYear { get; set; }

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

    public async Task<IActionResult> OnGetYearStatementsAsync(int categoryId, int year)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Categories = await client.GetFromJsonAsync<List<CategoryResponse>>("api/categories") ?? [];
        SelectedCategoryId = categoryId;
        SelectedCategoryName = Categories.FirstOrDefault(c => c.Id == categoryId)?.Name;
        SelectedYear = year;
        var response = await client.GetAsync(
            $"api/statements?CategoryIdsIncluded={categoryId}&DateFrom={year}-01-01&DateTo={year}-12-31&SortBy=Date&Direction=Desc");
        if (response.IsSuccessStatusCode)
            YearStatements = await response.Content.ReadFromJsonAsync<List<StatementResponse>>() ?? [];
        return Partial("_CategoryYearStatements", this);
    }
}
