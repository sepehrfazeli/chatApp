using Microsoft.AspNetCore.SignalR;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string userSessionId)
        {
            var timestamp = DateTime.Now.ToString("HH:mm");
            await Clients.All.SendAsync("ReceiveMessage", user, message, timestamp, userSessionId);
        }

        public async Task UpdateUserName(string newName, string userSessionId)
        {
            await Clients.Others.SendAsync("UserNameUpdated", newName, userSessionId);
        }
    }
}
