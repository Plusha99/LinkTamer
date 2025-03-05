using FluentValidation;
using LinkTamer.Application.Contracts.ShortenUrl;
using LinkTamer.Application.Contracts.Stats;
using LinkTamer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkTamer.Api.Controllers;

[ApiController]
public class UrlShortenerController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly IValidator<ShortenRequest> _shortenRequestValidator;

    public UrlShortenerController(IUrlShortenerService urlShortenerService, IValidator<ShortenRequest> shortenRequestValidator)
    {
        _urlShortenerService = urlShortenerService;
        _shortenRequestValidator = shortenRequestValidator;
    }

    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _shortenRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var shortUrl = await _urlShortenerService.ShortenUrlAsync(request.OriginalUrl);

        return Ok(new ShortenResponse(shortUrl));
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginal(string shortUrl, CancellationToken cancellationToken)
    {
        var originalUrl = await _urlShortenerService.GetOriginalUrlAsync(shortUrl);
        if (originalUrl == null)
        {
            return NotFound();
        }

        return Redirect(originalUrl);
    }

    [HttpGet("{shortCode}/stats")]
    public async Task<IActionResult> GetStats(string shortCode, CancellationToken cancellationToken)
    {
        var (shortUrl, clicks) = await _urlShortenerService.GetClickStatsAsync(shortCode);

        if (clicks < 0)
        {
            return NotFound();
        }

        return Ok(new StatsResponse(shortUrl, clicks));
    }
}
