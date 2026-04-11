using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.WebApp.Pages.Stats;

[Authorize]
public class MonthlyModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MonthlyModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<MonthlyBreakdownResponse> Months { get; set; } = [];
    public BreakdownDetailsResponse? Detail { get; set; }
    public int? SelectedMonth { get; set; }
    public int? SelectedYear { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IgnoreInitsAndTransfers { get; set; } = true;

    [BindProperty(SupportsGet = true)]
    public bool IgnoreLoans { get; set; } = true;

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Months = await client.GetFromJsonAsync<List<MonthlyBreakdownResponse>>(
            $"api/charts/GetMonthlyBreakdownData?ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}") ?? [];

        // Auto-select the most recent month
        if (Months.Count > 0)
        {
            SelectedMonth = Months[0].Month;
            SelectedYear = Months[0].Year;
            Detail = await client.GetFromJsonAsync<BreakdownDetailsResponse>(
                $"api/charts/GetBreakdownDataForMonth?month={SelectedMonth}&year={SelectedYear}&ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        }
    }

    public async Task<IActionResult> OnGetDetailAsync(int month, int year)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Detail = await client.GetFromJsonAsync<BreakdownDetailsResponse>(
            $"api/charts/GetBreakdownDataForMonth?month={month}&year={year}&ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        SelectedMonth = month;
        SelectedYear = year;
        return Partial("_MonthlyDetail", this);
    }
}
