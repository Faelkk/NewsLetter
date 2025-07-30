using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.DTOS.Users;

namespace Newsletter.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAsync();

    Task<UserDto> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task <UserDto> UpdateAsync(Guid id,UpdateUserRequest request);

  Task<bool> DeleteAsync(Guid id);

}

