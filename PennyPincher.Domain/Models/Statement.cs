using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PennyPincher.Domain.Models
{
    public class Statement
    {
        public int Id { get; set; }
        [Required] public DateTime Date { get; set; }
        [Required] public int AccountId { get; set; }
        [Required] [Column(TypeName = "decimal(8, 2)")] public decimal Amount { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public int CategoryId { get; set; }
        [Required] public string UserId { get; set; } = string.Empty;

        public virtual Category? Category { get; set; }
        public virtual Account? Account { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
