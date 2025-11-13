using ApiContracts.Auth;
using ApiContracts.Users;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        // Find user by username
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        // Check password
        if (user.Password != request.Password)
        {
            return Unauthorized("Invalid username or password");
        }

        // Return UserDto (without password)
        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Name = user.Name
        };

        return Ok(userDto);
    }
}

