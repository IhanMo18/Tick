using MediatR;
using Models.Profiles;
using Models.Profiles.Interface;

namespace Application.Profile.Command;

public record ChangeStatusCommand :IRequest<Guid>
{
    public ProfileStatus status;
    public Guid id;        
}

public sealed class ChangeStatusCommandHandler : IRequestHandler<ChangeStatusCommand, Guid>
{
    
    private readonly IPerfilRepository _repo;

    public ChangeStatusCommandHandler(IPerfilRepository repo) => _repo = repo;
    
    public async Task<Guid> Handle(ChangeStatusCommand cmd,CancellationToken cancellationToken)
    {
       var profile = await _repo.GetByIdAsync(cmd.id, cancellationToken);
       if(profile==null) throw new ArgumentException("Profile not found");  
       profile.ChangeStatus(cmd.status);
       await _repo.UpdateAsync(profile,cancellationToken);
       return cmd.id;
    }
}