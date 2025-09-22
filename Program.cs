using ChatApplication.Hubs;
using ChatApplication.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatContext>(options =>
    options.UseSqlite("Data Source=chat.db"));

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
                "http://localhost:5173",           // Vite dev server
                "http://localhost:3000",           // Local production
                "http://chatapp-frontend:3000",    // Docker Compose

                // for azure
                "https://sepehr-chatapp-frontend.happyforest-d52b3acf.westeurope.azurecontainerapps.io",
                "https://sepehr-chatapp-backend.happyforest-d52b3acf.westeurope.azurecontainerapps.io"
            )
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Force port configuration for Container Apps
var isContainerApp = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CONTAINER_APP_NAME"));
var port = isContainerApp ? "8080" : // Azure Container Apps
           Environment.GetEnvironmentVariable("PORT") ?? // General PORT variable
           Environment.GetEnvironmentVariable("WEBSITES_PORT") ?? // Azure App Service
           "5002"; // Local development default

Console.WriteLine($"Starting application on port: {port}");
var urls = $"http://0.0.0.0:{port}";
builder.WebHost.UseUrls(urls);

var app = builder.Build();

// Auto-create database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChatContext>();
    context.Database.EnsureCreated();
}

app.UseCors();
app.UseRouting();

// health check
app.MapGet("/", () => "Chat Backend is running!");
app.MapGet("/health", () => "OK");

app.MapHub<ChatHub>("/chatHub");

app.Run();