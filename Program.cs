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
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.WebHost.UseUrls("http://localhost:5002");

var app = builder.Build();

// Auto-create database on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChatContext>();
    context.Database.EnsureCreated();
}

app.UseCors();
app.UseRouting();
app.MapHub<ChatHub>("/chatHub");

app.Run();
