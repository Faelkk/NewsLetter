namespace Newsletter.Application.DTOS.Users;


public record UserDto(Guid Id, string Name, string Email, string Plan, List<string> Interests);

