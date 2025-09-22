using ChatApplication.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSignalR();

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173") // frontend port
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();
app.UseRouting();

// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
