namespace LinkTamer.Application.Contracts.Stats
{
    public class StatsResponse
    {
        public string ShortUrl { get; set; }
        public int Clicks { get; set; }

        public StatsResponse(string shortUrl, int clicks)
        {
            ShortUrl = shortUrl;
            Clicks = clicks;
        }
    }
}
