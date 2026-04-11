using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.WebApp.Pages.Stats;

[Authorize]
public class YearlyModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public YearlyModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<YearlyBreakdownResponse> Years { get; set; } = [];
    public BreakdownDetailsResponse? Detail { get; set; }
    public int? SelectedYear { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IgnoreInitsAndTransfers { get; set; } = true;

    [BindProperty(SupportsGet = true)]
    public bool IgnoreLoans { get; set; } = true;

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Years = await client.GetFromJsonAsync<List<YearlyBreakdownResponse>>(
            $"api/charts/GetYearlyBreakdownData?ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}") ?? [];

        if (Years.Count > 0)
        {
            SelectedYear = Years[0].Year;
            Detail = await client.GetFromJsonAsync<BreakdownDetailsResponse>(
                $"api/charts/GetBreakdownDataForYear?year={SelectedYear}&ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        }
    }

    public async Task<IActionResult> OnGetDetailAsync(int year)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Detail = await client.GetFromJsonAsync<BreakdownDetailsResponse>(
            $"api/charts/GetBreakdownDataForYear?year={year}&ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
        SelectedYear = year;
        return Partial("_YearlyDetail", this);
    }
}
