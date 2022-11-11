using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Api.Extensions
{
    public static class CacheExtension
    {
        public static  async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T value , TimeSpan? absoluteTime = null, TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteTime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpireTime;
            var json = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(key,json,options);
        } 

        public static  async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);

            if(json is null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static  async Task RemoveObjectAsync(this IDistributedCache cache, string key)
        {
            await cache.RemoveAsync(key);

        }
    
    }
}