using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaffleKing.Data.Models;

namespace RaffleKing.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<DrawModel> Draws { get; set; }
    public DbSet<PrizeModel> Prizes { get; set; }
    public DbSet<EntryModel> Entries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Override table names to singular
        builder.Entity<DrawModel>().ToTable("Draw");
        builder.Entity<PrizeModel>().ToTable("Prize");
        builder.Entity<EntryModel>().ToTable("Entry");
    }
}