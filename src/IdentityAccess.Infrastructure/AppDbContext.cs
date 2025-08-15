using IdentityAccess.Domain;
using Microsoft.EntityFrameworkCore;

namespace IdentityAccess.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Username).IsUnique();
        b.Entity<RefreshToken>().HasIndex(x => x.Token).IsUnique();
    }
}
