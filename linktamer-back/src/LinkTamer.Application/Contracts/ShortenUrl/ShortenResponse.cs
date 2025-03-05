namespace LinkTamer.Application.Contracts.ShortenUrl
{
    public class ShortenResponse
    {
        public string ShortUrl { get; set; }

        public ShortenResponse(string shortUrl)
        {
            ShortUrl = shortUrl;
        }
    }
}
