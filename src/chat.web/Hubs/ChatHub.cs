using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace chat.web.Hubs
{
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

        public override Task OnConnectedAsync()
        {
            logger.LogInformation("Connection successful");
            return Task.CompletedTask;
        }
    }
}