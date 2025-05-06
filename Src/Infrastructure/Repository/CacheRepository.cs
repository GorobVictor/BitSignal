using System.Text.Json;
using Infrastructure.Interface;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Repository;

public class CacheRepository(IDistributedCache cache) : ICacheRepository
{
    private static class Keys
    {
        public static string Notification(string coin) => "SignUpOtp:" + coin;
    }

    public async Task SetCoinNotification(string coin)
    {
        await cache.SetAsync(Keys.Notification(coin), true, TimeSpan.FromMinutes(5));
    }

    public async Task<bool> AnyCoinNotification(string coin)
    {
        return await cache.GetAsync<bool?>(Keys.Notification(coin)) ?? false;
    }
}

public static class CacheExtensions
{
    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var str = await cache.GetStringAsync(key);

        if (string.IsNullOrWhiteSpace(str)) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(str);
        }
        catch
        {
            return default;
        }
    }

    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan expiry)
    {
        if (value is null) return;

        try
        {
            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expiry
            });
        }
        catch
        {
            // ignore
        }
    }
}