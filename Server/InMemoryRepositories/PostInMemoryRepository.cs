using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new();

    public PostInMemoryRepository()
    {
        if (!posts.Any())
        {
            posts.AddRange(new[]
            {
                new Post { Id = 1, Title = "Hello World", Body = "First post body", UserId = 1 },
                new Post { Id = 2, Title = "Second Post", Body = "More content here", UserId = 2 },
                new Post { Id = 3, Title = "Random Thoughts", Body = "Some random text", UserId = 1 }
            });
        }
    }

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        var existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost == null)
        {
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove == null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post?> GetSingleAsync(int id)
    {
        var post = posts.SingleOrDefault(p => p.Id == id);

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}