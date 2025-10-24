namespace ApiContracts.Posts;

using ApiContracts.Comments;

public record PostDto
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public int UserId { get; init; }
    public string? AuthorUsername { get; init; }
    public IReadOnlyCollection<CommentDto>? Comments { get; init; }
}

public record CreatePostDto
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required int UserId { get; init; }
}

public record UpdatePostDto
{
    public string? Title { get; init; }
    public string? Body { get; init; }
    public int? UserId { get; init; }
}

public record GetPostsQuery
{
    public string? TitleContains { get; init; }
    public int? UserId { get; init; }
    public string? AuthorNameContains { get; init; }
    public bool IncludeComments { get; init; }
    public bool IncludeAuthor { get; init; }
    public int? Skip { get; init; }
    public int? Take { get; init; }
}
