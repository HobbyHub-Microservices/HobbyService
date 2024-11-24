using System.ComponentModel.DataAnnotations;

namespace HobbyService.Models;

public class Hobby
{
    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    public required User User { get; set; }
}