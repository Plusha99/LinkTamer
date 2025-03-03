namespace LinkTamer.Application.Interfaces;

public interface IUrlShortenerService
{
    Task<string> ShortenUrlAsync(string originalUrl);
    Task<string> GetOriginalUrlAsync(string shortUrl);
    Task<(string ShortUrl, int Clicks)> GetClickStatsAsync(string shortCode);
}
