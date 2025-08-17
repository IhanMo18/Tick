namespace Application.Abstractions;

public interface IIdentityService
{
    Task<Guid> CreateUserAsync(string email, string password, string role, CancellationToken ct);
    Task<bool> ConfirmEmailAsync(Guid userId, string token, CancellationToken ct);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct);
}