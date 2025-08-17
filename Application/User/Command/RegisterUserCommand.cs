using Application.Abstractions;
using MediatR;
using Models.Profiles;
using Models.Profiles.Interface;
using Models.Users;
using Models.Users.Interface;
using Models.Profiles.ProfileDomainService;
using Prefs = Models.Profiles.Preferences; 

namespace Application.User.Command;



public sealed record RegisterUserCommand(
    string Email,
    string Password, 
    ProfileDraft Draft,
    UserRole Role) : IRequest<Guid>;


public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IIdentityService _identity;
    private readonly IUserRepository _users;
    private readonly IPerfilRepository _profiles;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(IIdentityService identity, IUserRepository users,IPerfilRepository profiles,IUnitOfWork unitOfWork)
    {
        _identity = identity;
        _users = users;
        _unitOfWork = unitOfWork;
        _profiles = profiles;   
    }

    public async Task<Guid> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        var emailVo = Email.Create(cmd.Email);
        var identityId = await _identity.CreateUserAsync(emailVo.Value, cmd.Password, cmd.Role.ToString(), ct);
        var user = Models.Users.User.CreateFromIdentity(identityId, emailVo, cmd.Role,cmd.Draft);
        var profile = ProfileDomainService.CreateInitialProfile(user,cmd.Draft); 

        await _users.AddAsync(user, ct);
        await _profiles.AddAsync(profile, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return identityId;
    }
}


