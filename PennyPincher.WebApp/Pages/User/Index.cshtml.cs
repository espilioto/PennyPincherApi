using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PennyPincher.WebApp.Pages.User;

[Authorize]
public class IndexModel : PageModel
{
    public string Email => User.Identity?.Name ?? "";
}
