using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;
public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<User>> LoadAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private async Task SaveAsync(List<User> users)
    {
        string json = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<User> AddAsync(User user)
    {
        var users = await LoadAsync();
        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;
        user.Id = maxId + 1;
        users.Add(user);
        await SaveAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        var users = await LoadAsync();
        var idx = users.FindIndex(u => u.Id == user.Id);
        if (idx != -1)
        {
            users[idx] = user;
            await SaveAsync(users);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var users = await LoadAsync();
        users.RemoveAll(u => u.Id == id);
        await SaveAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        var users = await LoadAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public IQueryable<User> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        return users.AsQueryable();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var users = await LoadAsync();
        return users.FirstOrDefault(u => u.Username == username);
    }

    public async Task<object> GetAllAsync()
    {
        var users = await LoadAsync();
        return users;
    }
}
