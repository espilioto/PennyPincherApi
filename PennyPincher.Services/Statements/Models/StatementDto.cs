using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PennyPincher.Services.Categories.Models;

namespace PennyPincher.Services.Statements.Models
{
    public class StatementDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int AmountInCents { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }

        public CategoryDto? Category { get; set; }
    }
}
