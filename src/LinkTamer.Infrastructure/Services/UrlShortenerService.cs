using LinkTamer.Application.Interfaces;
using StackExchange.Redis;

namespace LinkTamer.Infrastructure.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly IDatabase _redisDb;

    public UrlShortenerService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<string> ShortenUrlAsync(string originalUrl)
    {
        string shortUrl = GenerateShortUrl();
        await _redisDb.StringSetAsync(shortUrl, originalUrl, TimeSpan.FromDays(365));
        return shortUrl;
    }

    public async Task<string?> GetOriginalUrlAsync(string shortUrl)
    {
        var originalUrl = await _redisDb.StringGetAsync(shortUrl);
        if (!originalUrl.HasValue) return null;

        await _redisDb.StringIncrementAsync($"stats:{shortUrl}");
        return originalUrl;
    }

    public async Task<int> GetClickStatsAsync(string shortUrl)
    {
        var clicks = await _redisDb.StringGetAsync($"stats:{shortUrl}");
        return clicks.HasValue ? (int)clicks : 0;
    }

    private string GenerateShortUrl()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "").Replace("+", "").Replace("/", "")
            .Substring(0, 8);
    }
}
