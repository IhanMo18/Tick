using Application.User.Dto;

namespace Application.Abstractions.Services;

public interface IUserService
{
    Task<Guid> RegisterUserAsync(UserDto user);
}