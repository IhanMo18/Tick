using Data.Context;
using Microsoft.EntityFrameworkCore;
using Models.Profiles.Interface;

namespace Data.Repository.Profile
{
    public class ProfileRepository : IPerfilRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Models.Profiles.Profile> _profiles;

        public ProfileRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _profiles = _context.Set<Models.Profiles.Profile>();
        }

        public async Task AddAsync(Models.Profiles.Profile profile, CancellationToken ct = default)
        {
            await _profiles.AddAsync(profile, ct);
        }

        public async Task<Models.Profiles.Profile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _profiles.FindAsync(new object?[] { id }, ct);
        }

        public Task UpdateAsync(Models.Profiles.Profile profile, CancellationToken ct = default)
        {
            _profiles.Update(profile);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await _profiles.AsNoTracking().AnyAsync(p => p.Id == id, ct);
        }

        public async Task<bool> ExistsAliasAsync(string alias, CancellationToken ct = default)
        {
            return await _profiles.AsNoTracking().AnyAsync(p => p.Alias.Value == alias, ct);
        }

        public async Task<bool> ExistsForOwnerAsync(Guid userId, CancellationToken ct = default)
        {
            // La intención típica es OwnerId == userId (no Id == userId)
            return await _profiles.AsNoTracking().AnyAsync(p => p.OwnerId == userId, ct);
        }
    }
}