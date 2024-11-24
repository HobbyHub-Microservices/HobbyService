using HobbyService.Models;
using Microsoft.EntityFrameworkCore;

namespace HobbyService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Hobby> Hobbies { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<User>()
            .HasMany(p => p.Hobbies)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);
        
        modelBuilder
            .Entity<Hobby>()
            .HasOne(p => p.User)
            .WithMany(p => p.Hobbies)
            .HasForeignKey(p => p.UserId);
            
    }
}