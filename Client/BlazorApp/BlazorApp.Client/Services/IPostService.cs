using ApiContracts.Posts;

namespace BlazorApp.Client.Services;

public interface IPostService
{
    Task<PostDto> AddPostAsync(CreatePostDto request);
    Task UpdatePostAsync(int id, UpdatePostDto request);
    Task<PostDto> GetSingleAsync(int id, bool includeComments = false, bool includeAuthor = false);
    Task<IEnumerable<PostDto>> GetManyAsync(string? titleContains = null, int? userId = null, string? authorNameContains = null, bool includeComments = false, bool includeAuthor = false);
    Task DeleteAsync(int id);
}

