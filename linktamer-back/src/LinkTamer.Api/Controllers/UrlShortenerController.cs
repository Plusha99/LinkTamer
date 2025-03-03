using LinkTamer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkTamer.Api.Controllers;

[ApiController]
[Route("")]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;

    public UrlShortenerController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OriginalUrl))
        {
            return BadRequest(new { message = "URL не может быть пустым." });
        }

        var shortUrl = await _urlShortenerService.ShortenUrlAsync(request.OriginalUrl);
        return Ok(new { shortUrl });
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginal(string shortUrl)
    {
        var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(shortUrl);
        if (originalUrl == null)
        {
            return NotFound();
        }
        
        return Redirect(originalUrl);
    }

    [HttpGet("stats/{shortCode}")]
    public async Task<IActionResult> GetStats(string shortCode)
    {
        var (shortUrl, clicks) = await _urlShortenerService.GetClickStatsAsync(shortCode);

        if (clicks == 0)
        {
            return NotFound();
        }

        return Ok(new { shortUrl, clicks });
    }
}
