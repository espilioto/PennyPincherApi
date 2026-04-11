using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PennyPincher.WebApp.Pages;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        Response.Cookies.Delete("jwt");
        return RedirectToPage("/Login");
    }
}
