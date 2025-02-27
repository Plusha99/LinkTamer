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
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OriginalUrl))
        {
            return BadRequest(new { message = "URL не может быть пустым." });
        }

        var shortUrl = await _urlShortenerService.ShortenUrlAsync(request.OriginalUrl);
        return Ok(new { shortUrl });
    }

    [HttpGet("redirect/{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginal(string shortUrl)
    {
        var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(shortUrl);
        if (originalUrl == null)
        {
            return NotFound();
        }
        
        return Redirect(originalUrl);
    }

    [HttpGet("stats/{shortUrl}")]
    public async Task<IActionResult> GetStats(string shortUrl)
    {
        var clicks = await _urlShortenerService.GetClickStatsAsync(shortUrl);

        if (clicks == null)
        {
            return NotFound();
        }

        return Ok(new { shortUrl, clicks });
    }
}
