using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> _comments = new();

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = _comments.Any() ? _comments.Max(c => c.Id) + 1 : 1;
        _comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        var existingComment = _comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment == null)
        {
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");
        }

        _comments.Remove(existingComment);
        _comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var commentToRemove = _comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove == null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        _comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        var comment = _comments.SingleOrDefault(c => c.Id == id);
        return comment == null ? throw new InvalidOperationException($"Comment with ID '{id}' not found") : Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return _comments.AsQueryable();
    } 
}