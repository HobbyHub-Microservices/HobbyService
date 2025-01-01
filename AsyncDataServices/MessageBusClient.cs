using System.Text;
using System.Text.Json;
using HobbyService.DTO;
using RabbitMQ.Client;

namespace HobbyService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration config)
    {
        _config = config;
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQHost"],
            Port = int.Parse(_config["RabbitMQPort"] ?? string.Empty),
            ClientProvidedName = "HobbyService"
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(exchange: "hobby.topic", type: ExchangeType.Topic, durable: true);
            _connection.ConnectionShutdown += RabbitMq_ConnectionShutDown;
            Console.WriteLine("--> Connected to RabbitMQ");
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not connect to message bus: {e.Message}");
        }
        
        
    }
    public void PublishNewPost(HobbyEditPublishDTO hobbyEditPublishDto)
    {
        var message = JsonSerializer.Serialize(hobbyEditPublishDto);
        
        if (_connection.IsOpen)
        {
            Console.WriteLine($"--> Sending message to RabbitMQ: {message}");

        }
        else
        {
            Console.WriteLine($"--> RabbitMQ is closed, not able to send message");
        }
    }
    
    public void HobbyEdited(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "hobby.topic",
            routingKey: "hobby.topic.edit",
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(message));
        Console.WriteLine($"--> Sent message to RabbitMQ Post: {message}");
    }

    public void HobbyDeleted(string message, string exchange, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(message));
        Console.WriteLine($"--> Sent message to RabbitMQ Post: {message}");
    }


    private void Dispose()
    {
        Console.WriteLine("--> Disposing of RabbitMQ");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private static void RabbitMq_ConnectionShutDown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }


    public void SendMessage_HobbyEdited(HobbyEditPublishDTO hobbyEditPublishDto)
    {
        var message = JsonSerializer.Serialize(hobbyEditPublishDto);
        
        if (_connection.IsOpen)
        {
            Console.WriteLine($"--> Sending message to RabbitMQ: {message}");
            HobbyEdited(message);
        }
        else
        {
            Console.WriteLine($"--> RabbitMQ is closed, not able to send message");
        }
    }

    public void SendMessage_HobbyQueryDeleted(HobbyDeleteQueryPublishDTO hobbyEditQueryPublishDto)
    {
        var message = JsonSerializer.Serialize(hobbyEditQueryPublishDto);
        
        if (_connection.IsOpen)
        {
            Console.WriteLine($"--> Sending message to RabbitMQ: {message}");
            HobbyDeleted(message, "hobby.query.topic", "hobby.topic.delete");
        }
        else
        {
            Console.WriteLine($"--> RabbitMQ is closed, not able to send message");
        }
    }

    public void SendMessage_HobbyCommandDeleted(HobbyDeleteCommandPublishDTO hobbyEditCommandPublishDto)
    {
        var message = JsonSerializer.Serialize(hobbyEditCommandPublishDto);
        
        
        if (_connection.IsOpen)
        {
            Console.WriteLine($"--> Sending message to RabbitMQ: {message}");
            HobbyDeleted(message, "hobby.command.topic", "hobby.topic.delete");
        }
        else
        {
            Console.WriteLine($"--> RabbitMQ is closed, not able to send message");
        }
    }
}