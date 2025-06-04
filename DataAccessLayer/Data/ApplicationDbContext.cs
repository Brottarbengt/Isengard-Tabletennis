using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Player> Players { get; set; } = null!;
    public virtual DbSet<Match> Matches { get; set; } = null!;
    public virtual DbSet<Set> Sets { get; set; } = null!;
    public virtual DbSet<PlayerMatch> PlayerMatches { get; set; } = null!;
    public virtual DbSet<SetInfo> SetInfos { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>()
            .Property(p => p.IsActive)
            .HasDefaultValue(true);
        modelBuilder.Entity<Player>()
            .Property(r => r.PlayerWinRatio)
            .HasPrecision(7, 4);
    }



}
