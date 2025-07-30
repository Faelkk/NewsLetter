using System.ComponentModel.DataAnnotations;

namespace Newsletter.Presentation.DTOS;

public record GenerateNewsletterRequest(
    [Required] Guid UserId,

    [Required]
    [MinLength(1, ErrorMessage = "Você deve informar pelo menos um tópico.")]
    List<string> Topics
);