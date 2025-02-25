using LinkTamer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkTamer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;

    public UrlShortenerController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] string originalUrl)
    {
        var shortUrl = await _urlShortenerService.ShortenUrlAsync(originalUrl);
        return Ok(new { shortUrl });
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectUrl(string shortUrl)
    {
        var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(shortUrl);
        if (originalUrl == null) return NotFound();

        return Ok(new { originalUrl });
    }

    [HttpGet("stats/{shortUrl}")]
    public async Task<IActionResult> GetStats(string shortUrl)
    {
        var clicks = await _urlShortenerService.GetClickStatsAsync(shortUrl);
        return Ok(new { shortUrl, clicks });
    }
}
