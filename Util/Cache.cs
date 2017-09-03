using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util
{
    public class Cache
    {
        private readonly IMemoryCache _cache;
        private const int baseTimeout = 240;

        public Cache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Set<T>(string key, T value, int minutesTimeout = baseTimeout)
        {
            _cache.Set<T>(key, value, TimeSpan.FromMinutes(minutesTimeout));
        }

        public T GetOrCreate<T>(string key, Func<T> populateFunc, int minutesTimeout = baseTimeout)
        {
            return _cache.GetOrCreate<T>(key, entry =>
                    {
                        entry.SlidingExpiration = TimeSpan.FromMinutes(minutesTimeout);
                        return populateFunc();
                    });
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
