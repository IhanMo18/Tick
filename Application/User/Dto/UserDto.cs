using Application.User.Command;
using Models.Profiles;
using Models.Users;

namespace Application.User.Dto;

 public record UserDto(string Email, string Password, UserRole Role,ProfileDraft ProfileDraft);