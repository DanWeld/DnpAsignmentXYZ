using ApiContracts.Users;

namespace BlazorApp.Client.Services;

public interface IUserService
{
    Task<UserDto> AddUserAsync(CreateUserDto request);
    Task UpdateUserAsync(int id, UpdateUserDto request);
    Task<UserDto> GetSingleAsync(int id);
    Task<IEnumerable<UserDto>> GetManyAsync(string? usernameContains = null);
    Task DeleteAsync(int id);
}

