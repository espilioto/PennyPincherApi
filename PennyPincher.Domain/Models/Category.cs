using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Required] public string UserId { get; set; } = null!;
        public int SortOrder { get; set; } = 0;

        public virtual IdentityUser? User { get; set; }
    }
}
