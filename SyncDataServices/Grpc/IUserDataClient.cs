using HobbyService.Models;

namespace HobbyService.SyncDataServices.Grpc;

public interface IUserDataClient
{
    IEnumerable<User> ReturnAllUsers();
}