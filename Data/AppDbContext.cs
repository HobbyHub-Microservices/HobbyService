using HobbyService.Models;
using Microsoft.EntityFrameworkCore;

namespace HobbyService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    
    public DbSet<Hobby> Hobbies { get; set; }

    
 
}