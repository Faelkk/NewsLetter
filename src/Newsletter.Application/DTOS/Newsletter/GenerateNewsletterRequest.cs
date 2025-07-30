namespace Newsletter.Presentation.DTOS;

public record GenerateNewsletterRequest(
    Guid UserId,
    List<string> Topics);
