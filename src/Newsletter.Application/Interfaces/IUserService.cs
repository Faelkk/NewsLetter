using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.DTOS.Users;

namespace Newsletter.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAsync();

    Task<UserDto?> GetByIdAsync(Guid id);
    Task<TokenDto> CreateAsync(CreateUserRequest request);
    
    Task<TokenDto> LoginAsync(LoginUserRequest request);
    Task <UserDto?> UpdateAsync(Guid id,UpdateUserRequest request);

  Task<bool> DeleteAsync(Guid id);

}

