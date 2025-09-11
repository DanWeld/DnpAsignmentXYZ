using Entities;
using RepositoryContacts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> _posts = new();

    public Task<Post> AddAsync(Post post)
    {
        post.Id = _posts.Any() ? _posts.Max(p => p.Id) + 1 : 1;
        _posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        var existingPost = _posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost == null)
        {
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
        }

        _posts.Remove(existingPost);
        _posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var postToRemove = _posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove == null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        _posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        var post = _posts.SingleOrDefault(p => p.Id == id);
        if (post == null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return _posts.AsQueryable();
    }
}