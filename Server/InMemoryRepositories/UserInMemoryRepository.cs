using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public Task<User> AddAsync(User user)
    {
        int maxId = _users.Count > 0 ? _users.Max(u => u.Id) : 0;
        user.Id = maxId + 1;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var idx = _users.FindIndex(u => u.Id == user.Id);
        if (idx != -1) _users[idx] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _users.RemoveAll(u => u.Id == id);
        return Task.CompletedTask;
    }

    public Task<User?> GetSingleAsync(int id)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    }

    public IQueryable<User> GetMany()
    {
        return _users.AsQueryable();
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Username == username));
    }

    public Task<object> GetAllAsync()
    {
        return Task.FromResult<object>(_users);
    }
}