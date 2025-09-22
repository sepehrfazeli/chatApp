using Microsoft.AspNetCore.SignalR;
using ChatApplication.Data;
using ChatApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatContext _context;

        public ChatHub(ChatContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            // Send recent messages to newly connected client
            var recentMessages = await _context.ChatMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(50) // Load last 50 messages
                .OrderBy(m => m.Timestamp) // Re-order chronologically
                .ToListAsync();

            foreach (var msg in recentMessages)
            {
                var timestamp = msg.Timestamp.ToString("HH:mm");
                await Clients.Caller.SendAsync("ReceiveMessage", msg.UserName, msg.Message, timestamp);
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            // Save message to database
            var chatMessage = new ChatMessage
            {
                UserName = user,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
            
            // Broadcast to all clients
            var timestamp = DateTime.Now.ToString("HH:mm");
            await Clients.All.SendAsync("ReceiveMessage", user, message, timestamp);
        }
    }
}
