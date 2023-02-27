using Microsoft.EntityFrameworkCore;
using PennyPincher.Domain.Models;

namespace Data
{
    public class PennyPincherApiDbContext : DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Statement> Statements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable(nameof(Categories));
            modelBuilder.Entity<Statement>().ToTable(nameof(Statements));
        }

        public PennyPincherApiDbContext(DbContextOptions<PennyPincherApiDbContext> options) : base(options)
        { }
    }
}
