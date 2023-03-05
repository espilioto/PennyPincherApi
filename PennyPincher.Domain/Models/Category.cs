using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string UserId { get; set; } = string.Empty;

        public virtual IdentityUser? User { get; set; }
    }
}
