using Data.Context;
using Microsoft.EntityFrameworkCore;
using Models.Users.Interface;

namespace Data.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Models.Users.User> _repository;

    public UserRepository(AppDbContext context)
    {
        _context = context;
        _repository = context.Set<Models.Users.User>();
    }

    public async Task AddAsync(Models.Users.User user, CancellationToken ct = default)
    {
        await _repository.AddAsync(user, ct);
    }

    public async Task<Models.Users.User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _repository.FindAsync(new object?[] { id }, ct);
    }

    public Task UpdateAsync(Models.Users.User user, CancellationToken ct = default)
    {
        _repository.Update(user);
        return Task.CompletedTask;
    }
}