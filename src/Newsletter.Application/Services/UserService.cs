using Newsletter.Application.DTOS.Users;
using Newsletter.Application.Interfaces;
using NewsLetter.Domain.Entities;

namespace Newsletter.Application.Services;

public class UserService : IUserService
{
    public Task<IEnumerable<UserDto>> GetAsync()
    {
        var users = new List<UserDto>
        {
            new UserDto(
                Id: Guid.NewGuid(),
                Name: "João Silva",
                Email: "joao@email.com",
                Plan: "Premium",
                Interests: new List<string> { "IA", "Finanças" }
            ),
            new UserDto(
                Id: Guid.NewGuid(),
                Name: "Maria Souza",
                Email: "maria@email.com",
                Plan: "Basic",
                Interests: new List<string> { "Esportes", "Tecnologia" }
            )
        };

        return Task.FromResult((IEnumerable<UserDto>)users);
    }

    public Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = new UserDto(
            Id: id,
            Name: "João Silva",
            Email: "joao@email.com",
            Plan: "Premium",
            Interests: new List<string> { "IA", "Finanças" }
        );

        return Task.FromResult(user);
    }

    public Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var newUser = new UserDto(
            Id: Guid.NewGuid(),
            Name: request.Name,
            Email: request.Email,
            Plan: request.Plan,
            Interests: request.Interests ?? new List<string>()
        );

        return Task.FromResult(newUser);
    }

    public Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var updatedUser = new UserDto(
            Id: id,
            Name: request.Name ?? "Nome Atualizado",
            Email: request.Email ?? "email@atualizado.com",
            Plan: request.Plan ?? "Basic",
            Interests: request.Interests ?? new List<string>()
        );

        return Task.FromResult(updatedUser);
    }

    public Task<bool> DeleteAsync(Guid id)
    {

        return Task.FromResult(true);
    }
}

