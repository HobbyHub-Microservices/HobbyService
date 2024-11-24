using HobbyService.AsyncDataServices;
using HobbyService.Data;
using HobbyService.EventProcessing;
using HobbyService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUserDataClient, UserDataClient>();
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseInMemoryDatabase("InMem")); 
builder.Services.AddScoped<IHobbyRepo, HobbyRepo>();

// *** Add Controllers ***
builder.Services.AddControllers();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// *** Enable routing for Controllers ***
app.MapControllers();

PrepDb.PrepPopulation(app);

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception during app startup: {ex.Message}");
}

