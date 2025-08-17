using Application.Abstractions.Services;
using Application.User.Command;
using Application.User.Dto;
using MediatR;

namespace Data.Services.UserService;

public class UserService: IUserService
{
    private IMediator _mediator;   
    
    public UserService(IMediator mediator)=> _mediator = mediator;
    
    public async Task<Guid> RegisterUserAsync(UserDto user)
    {
      return await _mediator.Send(new RegisterUserCommand(user.Email,user.Password,user.ProfileDraft,user.Role));
    }
}