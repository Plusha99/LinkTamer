namespace LinkTamer.Application.Interfaces;

public interface IUrlShortenerService
{
    Task<string> ShortenUrlAsync(string originalUrl);
    Task<string?> GetOriginalUrlAsync(string shortUrl);
    Task<int> GetClickStatsAsync(string shortUrl);
}
