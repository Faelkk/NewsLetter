using System.ComponentModel.DataAnnotations;

namespace Newsletter.Application.DTOS.Users;

public record LoginUserRequest( 
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
    string Email,
    
    [Required(ErrorMessage = "A password é obrigatória.")]
    [MinLength(8, ErrorMessage = "A password deve conter 8 caracteres no minimo.")]
    string Password);