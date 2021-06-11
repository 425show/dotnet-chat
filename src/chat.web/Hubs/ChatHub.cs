using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace chat.web.Hubs
{
    [Authorize]
    [RequiredScope("user.chat")]
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
                username = GetNameFromTokenClaims(this.Context)
            });
        }
        
        public override Task OnConnectedAsync()
        {
            var username = GetNameFromTokenClaims(this.Context);
            logger.LogInformation($"{username} just logged in and connected");
            return Task.CompletedTask;
        }

        private string GetNameFromTokenClaims(HubCallerContext context)
        {
            return context.User.Claims.FirstOrDefault(c => c.Type.Equals("Name", System.StringComparison.InvariantCultureIgnoreCase)).Value; 
        }
    }
}