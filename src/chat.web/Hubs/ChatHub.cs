using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace chat.web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("messageReceived", new {
                text = message,
                // username = this.Context.User.Identity.Name
            });
        }
    }
}