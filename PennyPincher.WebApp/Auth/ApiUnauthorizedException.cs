namespace PennyPincher.WebApp.Auth;

public class ApiUnauthorizedException : Exception
{
    public ApiUnauthorizedException()
        : base("The API returned 401 Unauthorized.") { }
}
