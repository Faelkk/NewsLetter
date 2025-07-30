using System.ComponentModel.DataAnnotations;

namespace Newsletter.Application.DTOS.Users;

public record UpdateUserRequest(
    [Required] string Name,

    [Required]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    string Email,

    [Required]
    [MinLength(1, ErrorMessage = "Informe pelo menos um interesse.")]
    List<string> Interests,

    [Required]
    string Plan
);