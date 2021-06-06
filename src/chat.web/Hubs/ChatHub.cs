using System.Collections.Generic;
using System.Threading.Tasks;
using chat.abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace chat.web.Hubs
{
    [Authorize]
    [RequiredScope("Chat")]
    public class ChatHub : Hub
    {
        internal const string ACTIVE_USERS = "activeUsers";
        private readonly ILogger<ChatHub> logger;
        private readonly AppCacheService appCacheService;

        public ChatHub(ILogger<ChatHub> logger, AppCacheService appCacheService)
        {
            this.appCacheService = appCacheService;
            this.logger = logger;
        }
        public async Task SignIn()
        {
            var username = this.Context.User.Identity.Name;
            logger.LogInformation($"{username} just logged in and connected");
            await Clients.All.SendAsync("userConnected", username);

            var activeUsers = this.appCacheService.MemoryCache.Get<List<string>>(ACTIVE_USERS);
            if (!activeUsers.Contains(username))
            {
                activeUsers.Add(username);
            }
            this.appCacheService.MemoryCache.Set<List<string>>(ACTIVE_USERS, activeUsers);
            await Clients.All.SendAsync("activeUserListUpdated", activeUsers);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var username = this.Context.User.Identity.Name;
            logger.LogInformation($"{username} just disconnected");

            await Clients.Others.SendAsync("userDisconnected", username);

            var activeUsers = this.appCacheService.MemoryCache.Get<List<string>>(ACTIVE_USERS);
            if (activeUsers.Contains(username))
            {
                activeUsers.Remove(username);
            }
            this.appCacheService.MemoryCache.Set<List<string>>(ACTIVE_USERS, activeUsers);
            await Clients.All.SendAsync("activeUserListUpdated", activeUsers);
        }

        public async Task SendPublicMessage(string message)
        {
            var username = this.Context.User.Identity.Name;
            logger.LogInformation($"{username} said {message}");
            await Clients.All.SendAsync("publicMessageReceived", new PublicMessage(
                message,
                new ChatUser
                {
                    Username = username,
                    DisplayName = username.Substring(0, username.IndexOf('@'))
                })
            );
        }
    }
}