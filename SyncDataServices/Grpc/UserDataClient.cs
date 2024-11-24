using AutoMapper;
using Grpc.Net.Client;
using HobbyService.Models;
using UserService;

namespace HobbyService.SyncDataServices.Grpc;

public class UserDataClient : IUserDataClient
{
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public UserDataClient(IConfiguration config, IMapper mapper)
    {
        _config = config;
        _mapper = mapper;
    }
    
    public IEnumerable<User> ReturnAllUsers()
    {
        Console.WriteLine($"--> Calling GRPC Service {_config["GrpcUser"]}");
        var channel = GrpcChannel.ForAddress(_config["GrpcUser"] ?? string.Empty);
        var client = new GrpcUser.GrpcUserClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllUsers(request);
            return _mapper.Map<IEnumerable<User>>(reply.User);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call GRPC Server {ex.Message}");
            return null;
        }
        
    }
}