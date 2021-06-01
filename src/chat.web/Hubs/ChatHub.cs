using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace chat.web.Hubs
{
    [Authorize]
    [RequiredScope("Chat")]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            this.logger = logger;
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("messageReceived", new {
                text = message,
                username = this.Context.User.Identity.Name
            });
        }

        public override async Task OnConnectedAsync()
        {
            var username = this.Context.User.Identity.Name;
            logger.LogInformation($"{username} just logged in and connected");
            await Clients.Others.SendAsync("userConnected", new {
                username = username
            });
        }
    }
}