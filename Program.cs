using HobbyService.AsyncDataServices;
using HobbyService.Data;
using HobbyService.EventProcessing;
using HobbyService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
    var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
    var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
    var dbPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    
    if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) ||
        string.IsNullOrEmpty(dbPassword))
    {
        
        Console.WriteLine("One of the string values for Postgres are empty");
        Console.WriteLine($"Host={dbHost};Port={dbPort};Database=Users;Username={dbUser};Password={dbPassword};Trust Server Certificate=true;");
        
    }
    
    builder.Configuration["ConnectionStrings:PostgressConn"] = $"Host={dbHost};Port={dbPort};Database=Hobbies;Username={dbUser};Password={dbPassword};Trust Server Certificate=true;";
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConn")));
    
    //Settings keycloak keys
    builder.Configuration["Keycloak:ClientId"] = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENTID");
    builder.Configuration["Keycloak:ClientSecret"] = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENTSECRET");
    
    builder.Configuration["Keycloak:Authority"] = Environment.GetEnvironmentVariable("KEYCLOAK_AUTHORITY");
    builder.Configuration["Keycloak:Audience"] = Environment.GetEnvironmentVariable("KEYCLOAK_AUDIENCE");
    builder.Configuration["Keycloak:AuthenticationURL"] = Environment.GetEnvironmentVariable("KEYCLOAK_AUTHENTICATION_URL");
    
    Console.WriteLine("Keycloak Configuration:");
    Console.WriteLine($"ClientId: {builder.Configuration["Keycloak:ClientId"]}");
    Console.WriteLine($"ClientSecret: {builder.Configuration["Keycloak:ClientSecret"]}"); // Be cautious with sensitive info
    Console.WriteLine($"Authority: {builder.Configuration["Keycloak:Authority"]}");
    Console.WriteLine($"Audience: {builder.Configuration["Keycloak:Audience"]}");
    Console.WriteLine($"AuthenticationURL: {builder.Configuration["Keycloak:AuthenticationURL"]}");

}
else
{
    Console.WriteLine("---> Using InMemory database");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMem")); 
    
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    Console.WriteLine("---> Using Keycloak stuff");
    Console.WriteLine(builder.Configuration["Keycloak:Authority"]);
    Console.WriteLine(builder.Configuration["Keycloak:Audience"]);
    
    options.Authority = builder.Configuration["Keycloak:Authority"]; // Keycloak realm URL
    options.Audience = builder.Configuration["Keycloak:Audience"];   // Client ID
    options.RequireHttpsMetadata = false;            // Disable for development
    
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Keycloak:Authority"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Keycloak:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    }; 
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"Token challenge triggered: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
    
});
builder.Services.AddAuthorization();
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");
// Add the Prometheus middleware
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

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