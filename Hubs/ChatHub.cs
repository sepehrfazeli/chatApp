using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm");
            await Clients.All.SendAsync("ReceiveMessage", user, message, timestamp);
        }
    }
}
