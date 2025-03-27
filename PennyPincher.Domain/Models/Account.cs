using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Domain.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Required] public string UserId { get; set; } = null!;
        [Required] public string ColorHex { get; set; } = "#000000";

        public virtual IdentityUser? User { get; set; }
    }
}
