using HobbyService.DTO;

namespace HobbyService.AsyncDataServices;

public interface IMessageBusSubscriber
{
    void PublishNewPost(HobbySendPublishedDto hobbyNamePublishedDto);
}