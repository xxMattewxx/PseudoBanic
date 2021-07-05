//CODE TAKEN FROM https://stackoverflow.com/questions/47874173/dictionary-cache-with-expiration-time
//CREDITS TO Hannen, Scott, 2017.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoBanic
{
    public class Cache<TKey, TValue>
    {
        public object lockObj = new object();
        private readonly Dictionary<TKey, CacheItem<TValue>> _cache = new Dictionary<TKey, CacheItem<TValue>>();

        public void Cleanup()
        {
            lock (lockObj)
            {
                List<TKey> toRemove = new List<TKey>();

                foreach (var aux in _cache)
                {
                    if (DateTimeOffset.Now - aux.Value.Created >= aux.Value.ExpiresAfter)
                        toRemove.Add(aux.Key);
                }

                foreach (var aux in toRemove)
                    _cache.Remove(aux);
            }
        }

        public Cache() {
            Task.Run(async () =>
            {
                while(true)
                {
                    Cleanup();
                    await Task.Delay(1000);
                }
            });
        }

        public void Store(TKey key, TValue value, TimeSpan expiresAfter)
        {
            _cache[key] = new CacheItem<TValue>(value, expiresAfter);
        }

        public TValue Get(TKey key)
        {
            if (!_cache.ContainsKey(key)) return default(TValue);

            var cached = _cache[key];
            if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
            {
                _cache.Remove(key);
                return default(TValue);
            }
            return cached.Value;
        }
    }
}
