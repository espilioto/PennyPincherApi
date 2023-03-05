using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Services.Categories.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }
    }
}
