using Models.Profiles;

namespace Models.Users.Interface;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task UpdateAsync(User user, CancellationToken ct = default);
}