using System.Text.Json;
using AutoMapper;
using HobbyService.Data;
using HobbyService.DTO;
using HobbyService.Models;

namespace HobbyService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEventType(message);
        switch (eventType)
        {
            case EventType.UserPublished:
                addUser(message);
                break;
            case EventType.Undetermined:
                Console.WriteLine(message);
                break;
                
       
        }
    }

    private EventType DetermineEventType(string notificationMessage)
    {
        Console.WriteLine("--> DetermineEventType");
     
        try
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        
            if (eventType?.Event == "User_Published")
            {
                Console.WriteLine("--> User_Published event detected");
                return EventType.UserPublished;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not determine event type: {ex.Message}");
        }

        Console.WriteLine("--> Unknown event type detected");
        return EventType.Undetermined;
    }

    private void addUser(string userPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IHobbyRepo>();
            var userPublishedEventDto = JsonSerializer.Deserialize<UserPublishedDto>(userPublishedMessage);

            try
            {
                var user = _mapper.Map<User>(userPublishedEventDto);
                if (!repo.ExternalUserExists(user.ExternalId))
                {
                    repo.CreateUser(user);
                    repo.SaveChanges();
                    Console.WriteLine("--> User added");
                }
                else
                {
                    Console.WriteLine("--> User already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add User to DB {ex.Message}");
            }
        }
    }

}
enum EventType
{
    UserPublished,
    Undetermined
}