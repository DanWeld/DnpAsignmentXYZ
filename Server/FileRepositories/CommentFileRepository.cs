using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;
public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Comment>> LoadAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();
    }

    private async Task SaveAsync(List<Comment> comments)
    {
        string json = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        var comments = await LoadAsync();
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 0;
        comment.Id = maxId + 1;
        comments.Add(comment);
        await SaveAsync(comments);
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        var comments = await LoadAsync();
        var idx = comments.FindIndex(c => c.Id == comment.Id);
        if (idx != -1)
        {
            comments[idx] = comment;
            await SaveAsync(comments);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var comments = await LoadAsync();
        comments.RemoveAll(c => c.Id == id);
        await SaveAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        var comments = await LoadAsync();
        return comments.FirstOrDefault(c => c.Id == id);
    }

    public IQueryable<Comment> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        var comments = JsonSerializer.Deserialize<List<Comment>>(json) ?? new List<Comment>();
        return comments.AsQueryable();
    }
}