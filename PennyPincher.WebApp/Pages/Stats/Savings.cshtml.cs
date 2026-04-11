using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.WebApp.Pages.Stats;

[Authorize]
public class SavingsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SavingsModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SavingsChartResponse? Data { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IgnoreInitsAndTransfers { get; set; } = true;

    [BindProperty(SupportsGet = true)]
    public bool IgnoreLoans { get; set; } = true;

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Data = await client.GetFromJsonAsync<SavingsChartResponse>(
            $"api/charts/GetSavingsRateChartData?ignoreInitsAndTransfers={IgnoreInitsAndTransfers}&ignoreLoans={IgnoreLoans}");
    }
}
