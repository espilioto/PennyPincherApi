using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Charts;
using PennyPincher.Contracts.Categories;

namespace PennyPincher.WebApp.Pages.Stats;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public MonthlyBreakdownResponse? LatestMonth { get; set; }
    public YearlyBreakdownResponse? CurrentYear { get; set; }
    public SavingsChartResponse? Savings { get; set; }
    public List<CategoryResponse> Categories { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public bool IgnoreInitsAndTransfers { get; set; } = true;

    [BindProperty(SupportsGet = true)]
    public bool IgnoreLoans { get; set; } = true;

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");

        var monthsTask = client.GetFromJsonAsync<List<MonthlyBreakdownResponse>>(
            $"api/charts/GetMonthlyBreakdownData?ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        var yearsTask = client.GetFromJsonAsync<List<YearlyBreakdownResponse>>(
            $"api/charts/GetYearlyBreakdownData?ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        var savingsTask = client.GetFromJsonAsync<SavingsChartResponse>(
            $"api/charts/GetSavingsRateChartData?ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        var categoriesTask = client.GetFromJsonAsync<List<CategoryResponse>>("api/categories");

        await Task.WhenAll(monthsTask, yearsTask, savingsTask, categoriesTask);

        var months = monthsTask.Result ?? [];
        var years = yearsTask.Result ?? [];

        LatestMonth = months.FirstOrDefault();
        CurrentYear = years.FirstOrDefault();
        Savings = savingsTask.Result;
        Categories = categoriesTask.Result ?? [];
    }
}
