using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Services.User.Models
{
    public class User
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
