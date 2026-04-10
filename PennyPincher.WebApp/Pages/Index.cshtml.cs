using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Charts;

namespace PennyPincher.WebApp.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<AccountResponse> Accounts { get; set; } = [];
    public List<GenericKeyValueResponse> BalanceChartData { get; set; } = [];

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");

        var accountsTask = client.GetFromJsonAsync<List<AccountResponse>>("api/accounts");
        var chartTask = client.GetFromJsonAsync<List<GenericKeyValueResponse>>("api/charts/GetOverviewBalanceChartData");

        await Task.WhenAll(accountsTask, chartTask);

        Accounts = accountsTask.Result ?? [];
        BalanceChartData = chartTask.Result ?? [];
    }
}
