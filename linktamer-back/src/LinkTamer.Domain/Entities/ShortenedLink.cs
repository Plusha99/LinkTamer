namespace LinkTamer.Domain.Entities
{
    public class ShortenedLink
    {
        public string Id { get; set; } = default!;
        public string OriginalUrl { get; set; } = default!;
        public string ShortenedUrl { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
