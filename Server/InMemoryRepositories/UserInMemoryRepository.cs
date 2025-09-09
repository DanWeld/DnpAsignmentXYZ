using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public Task<User> AddAsync(User user)
    {
        user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var existingUser = _users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");
        }

        _users.Remove(existingUser);
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var userToRemove = _users.SingleOrDefault(u => u.Id == id);
        if (userToRemove == null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        _users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        var user = _users.SingleOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany()
    {
        return _users.AsQueryable();
    }
}