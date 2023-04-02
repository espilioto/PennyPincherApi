using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Domain.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Required] public string UserId { get; set; } = null!;

        public virtual IdentityUser? User { get; set; }
    }
}
