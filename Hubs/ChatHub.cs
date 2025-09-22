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
            var recentMessages = await (from m in _context.ChatMessages
                                      join u in _context.Users on m.UserId equals u.Id
                                      orderby m.Timestamp descending
                                      select new { m.UserId, u.Name, m.Message, m.Timestamp })
                                      .Take(50)
                                      .OrderBy(m => m.Timestamp)
                                      .ToListAsync();

            foreach (var msg in recentMessages)
            {
                var timestamp = msg.Timestamp.ToString("HH:mm");
                await Clients.Caller.SendAsync("ReceiveMessage", msg.Name, msg.Message, timestamp, msg.UserId);
            }

            await base.OnConnectedAsync();
        }

        public async Task<int> JoinChat(string userName)
        {
            // Find existing user or create new one
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userName);
            
            if (user == null)
            {
                user = new User { Name = userName };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user.Id;
        }

        public async Task SendMessage(int userId, string message)
        {
            // Get user and save message
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            var chatMessage = new ChatMessage
            {
                UserId = userId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
            
            // Broadcast to all clients
            var timestamp = DateTime.Now.ToString("HH:mm");
            await Clients.All.SendAsync("ReceiveMessage", user.Name, message, timestamp, userId);
        }

        public async Task UpdateUserName(int userId, string newName)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            var oldName = user.Name;
            user.Name = newName;
            await _context.SaveChangesAsync();

            // Broadcast name change to all clients
            await Clients.All.SendAsync("UserNameUpdated", oldName, newName, userId);
        }
    }
}
