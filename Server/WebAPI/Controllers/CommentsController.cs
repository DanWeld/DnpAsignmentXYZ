using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _comments;
    private readonly IUserRepository _users;
    private readonly IPostRepository _posts;

    public CommentsController(ICommentRepository comments, IUserRepository users, IPostRepository posts)
    {
        _comments = comments;
        _users = users;
        _posts = posts;
    }

    // Create
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto request)
    {
        if (request.PostId is null) return BadRequest("PostId is required when creating a comment via /comments. Alternatively use /posts/{postId}/comments.");

        var post = await _posts.GetSingleAsync(request.PostId.Value);
        if (post == null) return BadRequest("Post with id " + request.PostId.Value + " does not exist.");

        var user = await _users.GetSingleAsync(request.UserId);
        if (user == null) return BadRequest("User with id " + request.UserId + " does not exist.");

        var comment = new Comment { Body = request.Body, PostId = request.PostId.Value, UserId = request.UserId };
        var created = await _comments.AddAsync(comment);
        return Created("/comments/" + created.Id, (object)ToDto(created, user.Username));
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CommentDto>> Update([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        var existing = await _comments.GetSingleAsync(id);
        if (existing == null) return NotFound();
        if (!string.IsNullOrWhiteSpace(request.Body)) existing.Body = request.Body!;
        await _comments.UpdateAsync(existing);

        var user = await _users.GetMany().FirstOrDefaultAsync(u => u.Id == existing.UserId);
        return Ok(ToDto(existing, user?.Username));
    }

    // Get single
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingle([FromRoute] int id)
    {
        var c = await _comments.GetSingleAsync(id);
        if (c == null) return NotFound();
        var user = await _users.GetMany().FirstOrDefaultAsync(u => u.Id == c.UserId);
        return Ok(ToDto(c, user?.Username));
    }

    // Get many with filters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetMany([FromQuery] GetCommentsQuery query)
    {
        var q = _comments.GetMany().AsQueryable();
        if (query.PostId.HasValue) q = q.Where(c => c.PostId == query.PostId.Value);
        if (query.UserId.HasValue) q = q.Where(c => c.UserId == query.UserId.Value);

        var users = await _users.GetMany().ToDictionaryAsync(u => u.Id, u => u.Username);

        if (!string.IsNullOrWhiteSpace(query.UsernameContains))
        {
            var needle = query.UsernameContains.ToUpperInvariant();
            var ids = users.Where(kv => (kv.Value ?? string.Empty).ToUpperInvariant().Contains(needle))
                .Select(kv => kv.Key).ToHashSet();
            q = q.Where(c => ids.Contains(c.UserId));
        }

        if (query.Skip is > 0) q = q.Skip(query.Skip.Value);
        if (query.Take is > 0) q = q.Take(query.Take.Value);

        var list = await q.ToListAsync();
        var dtos = list.Select(c => ToDto(c, users.GetValueOrDefault(c.UserId))).ToList();
        return Ok(dtos);
    }

    // Delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var existing = await _comments.GetSingleAsync(id);
        if (existing == null) return NotFound();
        await _comments.DeleteAsync(id);
        return NoContent();
    }

    private static CommentDto ToDto(Comment c, string? username) => new()
    {
        Id = c.Id,
        Body = c.Body,
        PostId = c.PostId,
        UserId = c.UserId,
        Username = username
    };
}