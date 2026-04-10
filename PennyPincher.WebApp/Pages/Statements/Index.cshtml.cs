using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Accounts;
using PennyPincher.Contracts.Categories;
using PennyPincher.Contracts.Statements;

namespace PennyPincher.WebApp.Pages.Statements;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<StatementResponse> Statements { get; set; } = [];
    public List<AccountResponse> Accounts { get; set; } = [];
    public List<CategoryResponse> Categories { get; set; } = [];

    // Filter properties
    [BindProperty(SupportsGet = true)]
    public List<int>? AccountIds { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<int>? CategoryIds { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DateFrom { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DateTo { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MinAmount { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxAmount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchText { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "date";

    [BindProperty(SupportsGet = true)]
    public string Direction { get; set; } = "desc";

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");

        var accountsTask = client.GetFromJsonAsync<List<AccountResponse>>("api/accounts");
        var categoriesTask = client.GetFromJsonAsync<List<CategoryResponse>>("api/categories");
        var statementsTask = FetchStatementsAsync(client);

        await Task.WhenAll(accountsTask, categoriesTask, statementsTask);

        Accounts = accountsTask.Result ?? [];
        Categories = categoriesTask.Result ?? [];
        Statements = statementsTask.Result;
    }

    public async Task<IActionResult> OnGetTableAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Statements = await FetchStatementsAsync(client);
        return Partial("_StatementRows", this);
    }

    public async Task<IActionResult> OnPostCreateAsync(
        DateTime date, decimal amount, int accountId, int categoryId, string description)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var request = new StatementRequest(date, accountId, amount, description ?? "", categoryId);
        var response = await client.PostAsJsonAsync("api/statements", request);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostEditAsync(
        int id, DateTime date, decimal amount, int accountId, int categoryId, string description)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var request = new StatementRequest(date, accountId, amount, description ?? "", categoryId);
        var response = await client.PutAsJsonAsync($"api/statements/{id}", request);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var response = await client.DeleteAsync($"api/statements/{id}");

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostMarkCheckedAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var response = await client.PutAsync("api/statements/markAllUncheckedNow", null);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    private async Task<List<StatementResponse>> FetchStatementsAsync(HttpClient client)
    {
        var queryParts = new List<string>
        {
            $"SortBy={SortBy}",
            $"Direction={Direction}"
        };

        if (AccountIds?.Count > 0)
        {
            foreach (var id in AccountIds)
                queryParts.Add($"AccountIdsIncluded={id}");
        }

        if (CategoryIds?.Count > 0)
        {
            foreach (var id in CategoryIds)
                queryParts.Add($"CategoryIdsIncluded={id}");
        }

        if (DateFrom.HasValue)
            queryParts.Add($"DateFrom={DateFrom:yyyy-MM-dd}");

        if (DateTo.HasValue)
            queryParts.Add($"DateTo={DateTo:yyyy-MM-dd}");

        if (MinAmount.HasValue)
            queryParts.Add($"MinAmount={MinAmount}");

        if (MaxAmount.HasValue)
            queryParts.Add($"MaxAmount={MaxAmount}");

        var query = string.Join("&", queryParts);
        var result = await client.GetFromJsonAsync<List<StatementResponse>>($"api/statements?{query}");

        // Client-side search text filter (not implemented server-side)
        if (!string.IsNullOrWhiteSpace(SearchText) && result != null)
        {
            result = result.Where(s =>
                s.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result ?? [];
    }
}
