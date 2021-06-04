using System.Collections.Generic;
using chat.web.Hubs;
using Microsoft.Extensions.Caching.Memory;

namespace chat.web
{
    public class AppCacheService
    {
        public AppCacheService(IMemoryCache memoryCache)
        {
            this.MemoryCache = memoryCache;
            this.MemoryCache.Set<List<string>>(ChatHub.ACTIVE_USERS, new List<string>());
        }
        public IMemoryCache MemoryCache { get; set; }
    }
}