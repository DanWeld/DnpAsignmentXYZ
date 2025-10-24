namespace ApiContracts.Comments;

public record CommentDto
{
    public int Id { get; init; }
    public required string Body { get; init; }
    public int PostId { get; init; }
    public int UserId { get; init; }
    public string? Username { get; init; }
}

public record CreateCommentDto
{
    public required string Body { get; init; }
    public required int UserId { get; init; }
    public int? PostId { get; init; }
}

public record UpdateCommentDto
{
    public string? Body { get; init; }
}

public record GetCommentsQuery
{
    public int? PostId { get; init; }
    public int? UserId { get; init; }
    public string? UsernameContains { get; init; }
    public int? Skip { get; init; }
    public int? Take { get; init; }
}
