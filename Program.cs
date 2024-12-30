using HobbyService.AsyncDataServices;
using HobbyService.Data;
using HobbyService.EventProcessing;
using HobbyService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConn")));
}
else
{
    Console.WriteLine("--> Using PostgreSQL Server");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConn")));
    
    // builder.Services.AddDbContext<AppDbContext>(opt => 
    //     opt.UseInMemoryDatabase("InMem")); 
}

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// builder.Services.AddScoped<IUserDataClient, UserDataClient>();
// builder.Services.AddDbContext<AppDbContext>(opt => 
//     opt.UseInMemoryDatabase("InMem")); 

builder.Services.AddScoped<IHobbyRepo, HobbyRepo>();

// *** Add Controllers ***
builder.Services.AddControllers();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
// builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Add the Prometheus middleware
app.UseRouting();

// Map the /metrics endpoint directly
app.MapMetrics(); // This maps the Prometheus metrics endpoint

// Map controllers or other routes directly
app.MapControllers(); // If you have any API controllers

// Optional: Add other middleware or configurations
app.UseHttpMetrics(); // Enables HTTP metrics

PrepDb.PrepPopulation(app);

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception during app startup: {ex.Message}");
}