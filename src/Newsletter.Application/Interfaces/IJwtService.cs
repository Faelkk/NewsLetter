using Newsletter.Domain.Entities;

namespace Newsletter.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}