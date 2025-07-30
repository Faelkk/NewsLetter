using Newsletter.Application.DTOS.Users;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<IEnumerable<UserDto>> GetAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserDto(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Plan: user.Plan,
            Interests: user.Interests.ToList()
        ));
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
    
        if (user is null)
            return null; 

        return new UserDto(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Plan: user.Plan,
            Interests: user.Interests.ToList()
        );
    }

    

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {  
        var existingUser = await _userRepository.GetByIdEmail(request.Email);
        if (existingUser is not null)
            throw new Exception("E-mail já cadastrado.");
        
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Plan = request.Plan ?? "free",
           Interests = request.Interests?.ToArray() ?? Array.Empty<string>()
        };

        var createdUser = await _userRepository.CreateAsync(user);

        return new UserDto(
            Id: createdUser.Id,
            Name: createdUser.Name,
            Email: createdUser.Email,
            Plan: createdUser.Plan,
            Interests: createdUser.Interests.ToList()
        );
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return null;

        user.Name = request.Name ?? user.Name;
        user.Email = request.Email ?? user.Email;
        user.Plan = request.Plan ?? user.Plan;

        if (request.Interests is not null && request.Interests.Any())
        {
            var mergedInterests = user.Interests
                .Union(request.Interests, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            user.Interests = mergedInterests;
        }

        var updatedUser = await _userRepository.UpdateAsync(user);

        return new UserDto(
            Id: updatedUser.Id,
            Name: updatedUser.Name,
            Email: updatedUser.Email,
            Plan: updatedUser.Plan,
            Interests: updatedUser.Interests.ToList()
        );
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}


