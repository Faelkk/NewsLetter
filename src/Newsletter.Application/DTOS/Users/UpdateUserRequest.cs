namespace Newsletter.Application.DTOS.Users;

public record UpdateUserRequest(string Name, string Email, List<string> Interests,string Plan);