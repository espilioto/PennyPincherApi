using Microsoft.AspNetCore.Authentication;

namespace PennyPincher.WebApp.Auth;

public class ApiUnauthorizedMiddleware
{
    private readonly RequestDelegate _next;

    public ApiUnauthorizedMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiUnauthorizedException)
        {
            if (context.Response.HasStarted)
                throw;

            await context.SignOutAsync("Cookies");
            context.Response.Cookies.Delete("jwt");

            if (context.Request.Headers.ContainsKey("HX-Request"))
            {
                context.Response.StatusCode = 200;
                context.Response.Headers["HX-Redirect"] = "/Login";
            }
            else
            {
                context.Response.Redirect("/Login");
            }
        }
    }
}
