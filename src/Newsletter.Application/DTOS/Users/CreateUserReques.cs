namespace Newsletter.Application.DTOS.Users;


public record CreateUserRequest(string Name, string Email, List<string> Interests,string Plan);
