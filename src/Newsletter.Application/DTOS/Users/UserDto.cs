using Newsletter.Domain.Enums;

namespace Newsletter.Application.DTOS.Users;


public record UserDto(Guid Id, string Name, string Email, List<string> Interests,  EnumRoles Role);

