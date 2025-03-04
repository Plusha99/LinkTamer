using LinkTamer.Application.Interfaces;
using LinkTamer.Domain.Entities;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace LinkTamer.Infrastructure.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly IDatabase _redisDb;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlShortenerService(IConnectionMultiplexer redis, IHttpContextAccessor httpContextAccessor)
    {
        _redisDb = redis.GetDatabase();
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> ShortenUrlAsync(string originalUrl)
    {
        var normalizerUrl = originalUrl.ToLower();

        var existingShortUrl = await _redisDb.HashGetAsync("url_mappings", normalizerUrl);
        if (!existingShortUrl.IsNullOrEmpty)
        {
            return existingShortUrl!;
        }

        var domain = GetDomain();
        var shortCode = GenerateShortUrl();
        var shortUrl = $"{domain}/{shortCode}";

        if (normalizerUrl.StartsWith(domain))
        {
            return originalUrl;
        }

        var link = new ShortenedLink
        {
            Id = shortCode,
            OriginalUrl = normalizerUrl,
            ShortenedUrl = shortUrl,
        };

        await _redisDb.HashSetAsync(shortCode, new HashEntry[]
        {
            new HashEntry("original_url", normalizerUrl),
            new HashEntry("short_url", shortUrl),
            new HashEntry("clicks", 0),
        });

        await _redisDb.HashSetAsync("url_mappings", normalizerUrl, shortUrl);

        return shortUrl;
    }

    public async Task<string> GetOriginalUrlAsync(string shortCode)
    {
        var data = await _redisDb.HashGetAllAsync(shortCode);

        if (data.Length == 0)
        {
            return null;
        }

        var fields = data.ToDictionary(entry => entry.Name.ToString(), entry => entry.Value.ToString());

        if (!fields.TryGetValue("original_url", out var originalUrl))
        {
            return null;
        }

        await _redisDb.HashIncrementAsync(shortCode, "clicks");

        return originalUrl;
    }

    public async Task<(string ShortUrl, int Clicks)> GetClickStatsAsync(string shortCode)
    {
        var data = await _redisDb.HashGetAllAsync(shortCode);

        if (data.Length == 0)
        {
            return (null, 0);
        }

        var fields = data.ToDictionary(entry => entry.Name.ToString(), entry => entry.Value.ToString());

        fields.TryGetValue("short_url", out var shortUrl);
        fields.TryGetValue("clicks", out var clicksStr);

        return (shortUrl, int.TryParse(clicksStr, out var clicks) ? clicks : 0);
    }

    private string GenerateShortUrl()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "").Replace("+", "").Replace("/", "")
            .Substring(0, 8);
    }

    private string GetDomain()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}
