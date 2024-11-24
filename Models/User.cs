using System.ComponentModel.DataAnnotations;

namespace HobbyService.Models;

public class User
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public required int ExternalId { get; set; }
    
    [Required]
    public required string Name { get; set; }
    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
}