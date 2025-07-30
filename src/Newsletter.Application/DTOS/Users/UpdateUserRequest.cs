using System.ComponentModel.DataAnnotations;

namespace Newsletter.Application.DTOS.Users;

public record UpdateUserRequest(
  string? Name,
  
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    string? Email,
  
    [MinLength(1, ErrorMessage = "Informe pelo menos um interesse.")]
    List<string>? Interests,
  
    string? Plan
);
