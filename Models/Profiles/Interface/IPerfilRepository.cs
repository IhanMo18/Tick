namespace Models.Profiles.Interface;

public interface IPerfilRepository
{
    Task AddAsync(Profile profile, CancellationToken ct = default);
    Task<Profile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Profile profile, CancellationToken ct = default);
    
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAliasAsync(string alias, CancellationToken ct = default);
    
    Task<bool>ExistsForOwnerAsync(Guid userId, CancellationToken ct = default);   
}