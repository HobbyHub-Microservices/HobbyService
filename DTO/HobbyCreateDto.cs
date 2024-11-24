using System.ComponentModel.DataAnnotations;

namespace HobbyService.DTO;

public class HobbyCreateDto
{
    [Required]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}