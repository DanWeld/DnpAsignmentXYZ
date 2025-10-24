namespace ApiContracts.Users;

public record UserDto
{
    public int Id { get; init; }
    public required string Username { get; init; }
    public string? Name { get; init; }
}

public record CreateUserDto
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public string? Name { get; init; }
}

public record UpdateUserDto
{
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? Name { get; init; }
}

public record GetUsersQuery
{
    public string? UsernameContains { get; init; }
    public string? NameContains { get; init; }
    public int? Skip { get; init; }
    public int? Take { get; init; }
}
