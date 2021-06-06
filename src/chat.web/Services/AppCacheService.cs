using System.Collections.Generic;
using chat.abstractions;
using chat.web.Hubs;
using Microsoft.Extensions.Caching.Memory;

namespace chat.web
{
    public class AppCacheService
    {
        public AppCacheService(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
            this.MemoryCache.Set<List<ChatUser>>(ChatHub.ACTIVE_USERS, new List<ChatUser>());
        }
        public IMemoryCache MemoryCache { get; set; }
    }
}