namespace Newsletter.Presentation.DTOS;

public record NewsletterDto(
    Guid Id,
    Guid UserId,
    List<string> Topics,
    string Content,
    bool Sent,
    DateTime CreatedAt); 