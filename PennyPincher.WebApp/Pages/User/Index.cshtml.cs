using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PennyPincher.Contracts.Users;

namespace PennyPincher.WebApp.Pages.User;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public string Email => User.Identity?.Name ?? "";

    public async Task<IActionResult> OnPostChangePasswordAsync([FromBody] ChangePasswordRequest request)
    {
        var client = _httpClientFactory.CreateClient("PennyPincherApi");
        var response = await client.PutAsJsonAsync("api/users/password", request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            return new ContentResult { Content = body, ContentType = "application/json", StatusCode = 400 };
        }

        return new JsonResult(new { success = true });
    }
}
