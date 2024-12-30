using HobbyService.DTO;

namespace HobbyService.AsyncDataServices;

public interface IMessageBusClient
{
    void SendMessage_HobbyEdited(HobbyEditPublishDTO hobbyEditPublishDto);
    void SendMessage_HobbyDeleted(HobbyDeletePublishDTO hobbyEditPublishDto);
}