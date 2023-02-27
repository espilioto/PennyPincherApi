using System.ComponentModel.DataAnnotations;

namespace PennyPincher.Domain.Models
{
    public class Statement
    {
        public int Id { get; set; }
        [Required] public DateTime Date { get; set; }
        [Required] public int AmountInCents { get; set; }
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }
    }
}
