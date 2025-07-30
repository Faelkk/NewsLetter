using Newsletter.Domain.Entities;

namespace Newsletter.Domain.Interfaces;

public interface IUserRepository
{


    public Task<IEnumerable<User>> GetAllAsync();

    public Task<User?> GetByIdAsync(Guid id);

    public Task<User?> GetByIdEmail(string email);

    public Task<User> CreateAsync(User user);

    public Task<User> UpdateAsync(User user);

    public Task<bool> DeleteAsync(Guid id);
}