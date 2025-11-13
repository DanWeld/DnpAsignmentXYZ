using ApiContracts.Comments;

namespace BlazorApp.Client.Services;

public interface ICommentService
{
    Task<CommentDto> AddCommentAsync(CreateCommentDto request);
    Task<CommentDto> AddCommentToPostAsync(int postId, CreateCommentDto request);
    Task UpdateCommentAsync(int id, UpdateCommentDto request);
    Task<CommentDto> GetSingleAsync(int id);
    Task<IEnumerable<CommentDto>> GetManyAsync(int? postId = null, int? userId = null, string? usernameContains = null);
    Task DeleteAsync(int id);
}

