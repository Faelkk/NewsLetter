using Newsletter.Application.DTOS.Users;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Enums;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher,IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }
    
    public async Task<IEnumerable<UserDto>> GetAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserDto(
            Id: user.Id,
            Name: user.Name,
            Email: user.Email,
            Role: user.Role,

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
            Interests: user.Interests.ToList(),
            Role: user.Role
        );
    }

    

    public async Task<TokenDto> CreateAsync(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetByIdEmail(request.Email);
        if (existingUser is not null)
            throw new Exception("E-mail já cadastrado.");

        var id = Guid.NewGuid();

        var hashedPassword = _passwordHasher.HashPassword(null!, request.Password);

        var user = new User
        {
            Id = id,
            Name = request.Name,
            Email = request.Email,
            Interests = request.Interests?.ToArray() ?? Array.Empty<string>(),
            Password = hashedPassword,
            Role = EnumRoles.User,
        };

        var createdUser = await _userRepository.CreateAsync(user);

        var token = _jwtService.GenerateToken(user); 

        return new TokenDto(token); 
    }


    public async Task<TokenDto> LoginAsync(LoginUserRequest request)
    {
        var user = await _userRepository.GetByIdEmail(request.Email);
        if (user == null)
            throw new Exception("Usuário não encontrado.");

        var isValidPassword = _passwordHasher.VerifyPassword(user, request.Password);
        if (!isValidPassword)
            throw new Exception("Usuário ou senha inválidos.");

        var token = _jwtService.GenerateToken(user);

        return new TokenDto(token);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return null;

        var updatedInterests = user.Interests;
        if (request.Interests is not null && request.Interests.Any())
        {
            updatedInterests = user.Interests
                .Union(request.Interests, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
        
        var updatedPassword = user.Password;
        if (!string.IsNullOrEmpty(request.Password))
        {
            updatedPassword = _passwordHasher.HashPassword(user, request.Password);
        }

        var updatedUser = new User
        {
            Id = user.Id,
            Name = request.Name ?? user.Name,
            Email = request.Email ?? user.Email,
            Password = updatedPassword,
            Interests = updatedInterests,
            Role = request.Role ?? user.Role
        };

        var resultUser = await _userRepository.UpdateAsync(updatedUser);

        return new UserDto(
            Id: resultUser.Id,
            Name: resultUser.Name,
            Email: resultUser.Email,
            Interests: resultUser.Interests.ToList(),
            Role: resultUser.Role
        );
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }
    

}



