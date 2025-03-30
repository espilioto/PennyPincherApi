using Microsoft.AspNetCore.Identity;
using PennyPincher.Domain.Models;
using PennyPincher.Services.Categories.Models;

namespace PennyPincher.Services.Statements.Models
{
    public class StatementDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string UserId { get; set; } = string.Empty;

        public CategoryDto? Category { get; set; }
        public Account? Account { get; set; }
        public IdentityUser? User { get; set; }
    }
}
