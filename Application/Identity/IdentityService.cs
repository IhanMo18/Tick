using Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Application.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser<Guid>> _users;
    private readonly RoleManager<IdentityRole<Guid>> _roles;

    public IdentityService(UserManager<IdentityUser<Guid>> users, RoleManager<IdentityRole<Guid>> roles)
    {
        _users = users;
        _roles = roles;
    }

    public async Task<Guid> CreateUserAsync(string email, string password, string role, CancellationToken ct)
    {
        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            EmailConfirmed = false
        };

        var create = await _users.CreateAsync(user, password);
        if (!create.Succeeded)
            throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

        if (!await _roles.RoleExistsAsync(role))
            await _roles.CreateAsync(new IdentityRole<Guid>(role));

        await _users.AddToRoleAsync(user, role);
        return user.Id;
    }
    public async Task<bool> ConfirmEmailAsync(Guid userId, string token, CancellationToken ct)
    {
        var user = await _users.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _users.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct)
    {
        var user = await _users.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _users.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }
}