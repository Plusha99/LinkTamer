using LinkTamer.Application.Interfaces;
using LinkTamer.Domain.Entities;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System.Text.Json;

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
        string domain = GetDomain();
        if (originalUrl.StartsWith(domain))
        {
            return originalUrl;
        }

        string existingShortUrl = await _redisDb.HashGetAsync("url_mappings", originalUrl);
        if (!string.IsNullOrEmpty(existingShortUrl))
        {
            return existingShortUrl;
        }

        string shortCode = GenerateShortUrl();
        string shortUrl = $"{domain}/{shortCode}";

        var link = new ShortenedLink
        {
            Id = shortCode,
            OriginalUrl = originalUrl,
            ShortenedUrl = shortUrl,
            CreatedAt = DateTime.UtcNow
        };

        string json = JsonSerializer.Serialize(link);

        await _redisDb.HashSetAsync("url_mappings", originalUrl, shortUrl);

        await _redisDb.StringSetAsync(shortCode, json, TimeSpan.FromDays(365));

        return shortUrl;
    }

    public async Task<string> GetOriginalUrlAsync(string shortCode)
    {
        var data = await _redisDb.StringGetAsync(shortCode);

        if (!data.HasValue)
        {
            return null;
        }

        var link = JsonSerializer.Deserialize<ShortenedLink>(data!);
        if (link == null)
        {
            return null;
        }

        await _redisDb.StringIncrementAsync($"stats:{shortCode}");
        return link.OriginalUrl;
    }

    public async Task<(string ShortUrl, int Clicks)> GetClickStatsAsync(string shortCode)
    {
        var clicks = await _redisDb.StringGetAsync($"stats:{shortCode}");
        var data = await _redisDb.StringGetAsync(shortCode); 

        if (!data.HasValue) return (null, 0);

        var link = JsonSerializer.Deserialize<ShortenedLink>(data!);
        return (link?.ShortenedUrl, clicks.HasValue ? (int)clicks : 0);
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
