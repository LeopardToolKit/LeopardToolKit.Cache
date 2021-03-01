using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace LeopardToolKit.Cache
{
    public class LeopardMemoryCache : ICache
    {
        private readonly static JsonSerializerOptions _serializerOptions =
            new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        private readonly ConcurrentDictionary<string, CacheItem> _store;
        private DateTimeOffset LastScanTime = DateTimeOffset.Now;
        private SemaphoreSlim scaningLock = new SemaphoreSlim(1, 1);

        public LeopardMemoryCache(string categoryName)
        {
            _store = new ConcurrentDictionary<string, CacheItem>();
            this.CategoryName = categoryName;
        }

        public string CategoryName { get; }

        public T Get<T>(string key)
        {
            if(_store.TryGetValue(key, out CacheItem cacheItem))
            {
                if(typeof(T) == typeof(string))
                {
                    return (T)(object)cacheItem.Json;
                }

                if (string.IsNullOrEmpty(cacheItem.Json) || cacheItem.ExpiredTime< DateTimeOffset.Now)
                {
                    Remove(key);
                    TryScanExpiredCacheItem();
                    return default;
                }
                else
                {
                    TryScanExpiredCacheItem();
                    return JsonSerializer.Deserialize<T>(cacheItem.Json, _serializerOptions);
                }
            }
            else
            {
                TryScanExpiredCacheItem();
                return default;
            }
            
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            var json = JsonSerializer.Serialize(value, _serializerOptions);
            _store.TryAdd(key, new CacheItem { ExpiredTime = DateTimeOffset.Now.Add(absoluteExpirationTime), Json = json });
            TryScanExpiredCacheItem();
        }

        public void Remove(string key)
        {
            _store.TryRemove(key, out _);
            TryScanExpiredCacheItem();
        }

        private void TryScanExpiredCacheItem()
        {
            if(LastScanTime.AddMinutes(2) < DateTimeOffset.Now)
            {
                if (scaningLock.Wait(1000 * 2))
                {
                    Task.Factory.StartNew(delegate (object state)
                    {
                        var cache = (LeopardMemoryCache)state;
                        try
                        {
                            ScanExpiredCacheItem(cache);
                        }
                        finally
                        {
                            cache.scaningLock.Release();
                        }
                    }, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                }
            }
        }

        private static void ScanExpiredCacheItem(LeopardMemoryCache memoryCache)
        {

            var expiredKeys = memoryCache._store.Where(kvp => kvp.Value.ExpiredTime < DateTimeOffset.Now).Select(kvp => kvp.Key).ToList();
            foreach (var key in expiredKeys)
            {
                memoryCache.Remove(key);
            }
        }
    }

    internal class CacheItem
    {
        public string Json { get; set; }

        public DateTimeOffset ExpiredTime { get; set; }
    }
}
