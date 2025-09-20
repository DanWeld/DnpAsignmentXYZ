using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new();

    public UserInMemoryRepository()
    {
        if (!users.Any())
        {
            users.AddRange(new []
            {
                new User { Id = 1, Username = "DW", Password = "pw1", Name = "Daniel" },
                new User { Id = 2, Username = "KS", Password = "pw2", Name = "Kelsang" },
                new User { Id = 3, Username = "JH", Password = "pw3", Name = "Jwan" },
            });
        }
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        var existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var userToRemove = users.SingleOrDefault(u => u.Id == id);
        if (userToRemove == null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        var user = users.SingleOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }

    public Task<User?> GetByUsernameAsync(string empty)
    {
        var user = users.SingleOrDefault(u => u.Username == empty);
        return Task.FromResult(user);
        
    }

    public Task<object> GetAllAsync()
    {
        return Task.FromResult((object)users);
    }
}