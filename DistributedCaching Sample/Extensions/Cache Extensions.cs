using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DistributedCaching_Sample.Extensions
{
    public static class CacheExtensions
    {
        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static async Task<T> GetCacheValueAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            string result = await cache.GetStringAsync(key);
            if (String.IsNullOrEmpty(result))
            {
                return null;
            }
            var deserializedObj = JsonConvert.DeserializeObject<T>(result);
            return deserializedObj;
        }

        public static async Task SetCacheValueAsync<T>(this IDistributedCache cache, string key, T value) where T : class
        {
            DistributedCacheEntryOptions cacheEntryOptions = new();

            // Remove item from cache after duration
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);

            // Remove item from cache if unsued for the duration
            cacheEntryOptions.SlidingExpiration = TimeSpan.FromSeconds(30);

            string result = value.ToJsonString();

            await cache.SetStringAsync(key, result);
        }
    }
}
