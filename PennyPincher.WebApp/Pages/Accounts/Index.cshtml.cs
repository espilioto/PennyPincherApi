using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Accounts;

namespace PennyPincher.WebApp.Pages.Accounts;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public List<AccountResponse> Accounts { get; set; } = [];

    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        Accounts = await client.GetFromJsonAsync<List<AccountResponse>>("api/accounts") ?? [];
    }

    public async Task<IActionResult> OnPostCreateAsync(string name, string colorHex)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var request = new AccountRequest(name, "", colorHex);
        var response = await client.PostAsJsonAsync("api/accounts", request);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostEditAsync(int id, string name, string colorHex)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var request = new AccountRequest(name, "", colorHex);
        var response = await client.PutAsJsonAsync($"api/accounts/{id}", request);

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var response = await client.DeleteAsync($"api/accounts/{id}");

        if (!response.IsSuccessStatusCode)
            return BadRequest();

        return new JsonResult(new { success = true });
    }
}
