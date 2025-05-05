using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Domain.Models;

namespace PennyPincher.Data;

public class PennyPincherApiDbContext : IdentityUserContext<IdentityUser>
{
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Statement> Statements { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().ToTable(nameof(Categories));
        modelBuilder.Entity<Statement>().ToTable(nameof(Statements));
        modelBuilder.Entity<Account>().ToTable(nameof(Accounts));
    }

    public PennyPincherApiDbContext(DbContextOptions<PennyPincherApiDbContext> options) : base(options)
    { }
}
