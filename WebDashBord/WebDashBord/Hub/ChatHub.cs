
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;


namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task Sendint(string user, int message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message.ToString());
        }
        public async Task PushJson(string json)
        {
            await Clients.All.SendAsync("JsonGet", json);
        }
    }
}
