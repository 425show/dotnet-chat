using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Web.Resource;

namespace chat.web.Hubs
{
    [Authorize]
    [RequiredScope("Chat")]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("messageReceived", new {
                text = message,
                username = this.Context.User.Identity.Name
            });
        }
    }
}