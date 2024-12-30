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
    private readonly IHobbyRepo _hobbyRepo;

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
                // addUser(message);
                break;
            case EventType.Undetermined:
                Console.WriteLine(message);
                break;
            case EventType.PostPublished:
                Console.WriteLine(message);
                SendHobbyToPost(message);
                break;
       
        }
    }

    private void SendHobbyToPost(string message)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IHobbyRepo>();
            var hobbyNamePublishedDTO = JsonSerializer.Deserialize<HobbyNamePublishedDTO>(message);
        
            try
            {
               repo.SendHobbyNameToPost(hobbyNamePublishedDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add User to DB {ex.Message}");
            }
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
            
            if (eventType?.Event == "Post_Published")
            {
                Console.WriteLine("--> Post_Published event detected");
                return EventType.PostPublished; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not determine event type: {ex.Message}");
        }

        Console.WriteLine("--> Unknown event type detected");
        return EventType.Undetermined;
    }

    // private void addUser(string userPublishedMessage)
    // {
    //     using (var scope = _serviceScopeFactory.CreateScope())
    //     {
    //         var repo = scope.ServiceProvider.GetRequiredService<IHobbyRepo>();
    //         var userPublishedEventDto = JsonSerializer.Deserialize<UserPublishedDto>(userPublishedMessage);
    //
    //         try
    //         {
    //             var user = _mapper.Map<User>(userPublishedEventDto);
    //             if (!repo.ExternalUserExists(user.ExternalId))
    //             {
    //                 repo.CreateUser(user);
    //                 repo.SaveChanges();
    //                 Console.WriteLine("--> User added");
    //             }
    //             else
    //             {
    //                 Console.WriteLine("--> User already exists");
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Could not add User to DB {ex.Message}");
    //         }
    //     }
    // }

}
enum EventType
{
    UserPublished,
    Undetermined,
    PostPublished
}