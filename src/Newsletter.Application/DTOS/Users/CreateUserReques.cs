using System.ComponentModel.DataAnnotations;

namespace Newsletter.Application.DTOS.Users;

public record CreateUserRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Name,

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
    string Email,

    [Required(ErrorMessage = "Informe pelo menos um interesse.")]
    [MinLength(1, ErrorMessage = "Informe pelo menos um interesse.")]
    List<string> Interests,

    [Required(ErrorMessage = "O plano é obrigatório.")]
    string Plan
);