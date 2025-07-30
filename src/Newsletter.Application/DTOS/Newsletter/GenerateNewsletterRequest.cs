using System.ComponentModel.DataAnnotations;

namespace Newsletter.Presentation.DTOS;

public record GenerateNewsletterRequest(
    [Required] Guid UserId
    
);