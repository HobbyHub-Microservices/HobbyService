namespace HobbyService.DTO;

public class HobbyEditPublishDTO
{
    public required string Name { get; set; }
    public string Event { get; set; } 
    
    public string Name_old { get; set; } = string.Empty;
}