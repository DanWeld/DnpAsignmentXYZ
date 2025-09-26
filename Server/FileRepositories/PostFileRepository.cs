using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;
public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Post>> LoadAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
    }

    private async Task SaveAsync(List<Post> posts)
    {
        string json = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Post> AddAsync(Post post)
    {
        var posts = await LoadAsync();
        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;
        post.Id = maxId + 1;
        posts.Add(post);
        await SaveAsync(posts);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        var posts = await LoadAsync();
        var idx = posts.FindIndex(p => p.Id == post.Id);
        if (idx != -1)
        {
            posts[idx] = post;
            await SaveAsync(posts);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var posts = await LoadAsync();
        posts.RemoveAll(p => p.Id == id);
        await SaveAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        var posts = await LoadAsync();
        return posts.FirstOrDefault(p => p.Id == id);
    }

    public IQueryable<Post> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        var posts = JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
        return posts.AsQueryable();
    }
}