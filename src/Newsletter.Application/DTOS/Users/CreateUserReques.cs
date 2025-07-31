using System.ComponentModel.DataAnnotations;

namespace Newsletter.Application.DTOS.Users;

public record CreateUserRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Name,

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
    string Email,
    
    [Required(ErrorMessage = "A password é obrigatória.")]
    [MinLength(8, ErrorMessage = "A password deve conter 8 caracteres no minimo.")]
    string Password,
    

    [Required(ErrorMessage = "Informe pelo menos um interesse.")]
    [MinLength(1, ErrorMessage = "Informe pelo menos um interesse.")]
    List<string> Interests
    
);