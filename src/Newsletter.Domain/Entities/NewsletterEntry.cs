namespace Newsletter.Domain.Entities;

public class NewsletterEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<string> Topics { get; set; } = [];
    public string Content { get; set; } = null!;
    public bool Sent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
