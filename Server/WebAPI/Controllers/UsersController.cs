
using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;

    public UsersController(IUserRepository users)
    {
        _users = users;
    }

    // Create
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto request)
    {
        // Basic uniqueness check
        var existing = await _users.GetByUsernameAsync(request.Username);
        if (existing != null) return Conflict("Username '" + request.Username + "' is already taken.");

        var toCreate = new User
        {
            Username = request.Username,
            Password = request.Password,
            Name = request.Name ?? string.Empty
        };

        var created = await _users.AddAsync(toCreate);

        var dto = ToDto(created);
        return Created("/users/" + dto.Id, dto);
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        var existing = await _users.GetSingleAsync(id);
        if (existing == null) return NotFound();

        // If username is changed, ensure uniqueness
        if (!string.IsNullOrWhiteSpace(request.Username) && !string.Equals(request.Username, existing.Username, StringComparison.Ordinal))
        {
            var other = await _users.GetByUsernameAsync(request.Username);
            if (other != null && other.Id != id) return Conflict($"Username '{request.Username}' is already taken.");
            existing.Username = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Password)) existing.Password = request.Password!;
        if (!string.IsNullOrWhiteSpace(request.Name)) existing.Name = request.Name!;

        await _users.UpdateAsync(existing);
        return Ok(ToDto(existing));
    }

    // Get single
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle([FromRoute] int id)
    {
        var user = await _users.GetSingleAsync(id);
        if (user == null) return NotFound();
        return Ok(ToDto(user));
    }

    // Get many with filters
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany([FromQuery] GetUsersQuery query)
    {
        var q = _users.GetMany().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.UsernameContains))
            q = q.Where(u => (u.Username ?? string.Empty).IndexOf(query.UsernameContains, StringComparison.OrdinalIgnoreCase) >= 0);

        if (!string.IsNullOrWhiteSpace(query.NameContains))
            q = q.Where(u => (u.Name ?? string.Empty).IndexOf(query.NameContains, StringComparison.OrdinalIgnoreCase) >= 0);

        if (query.Skip is > 0) q = q.Skip(query.Skip.Value);
        if (query.Take is > 0) q = q.Take(query.Take.Value);

        var list = q.Select(ToDto).ToList();
        return Ok(list);
    }

    // Delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        // Optionally check existence
        var existing = await _users.GetSingleAsync(id);
        if (existing == null) return NotFound();

        await _users.DeleteAsync(id);
        return NoContent();
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Name = u.Name
    };
}