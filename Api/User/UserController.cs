using Application.Abstractions.Services;
using Application.User.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.User;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register-user")]
    public async Task<ActionResult<Guid>> RegisterUser([FromBody] UserDto user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _userService.RegisterUserAsync(user);
        return Ok(id);
    }
}