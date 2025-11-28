using ApiContracts.Comments;
using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _posts;
    private readonly IUserRepository _users;
    private readonly ICommentRepository _comments;

    public PostsController(IPostRepository posts, IUserRepository users, ICommentRepository comments)
    {
        _posts = posts;
        _users = users;
        _comments = comments;
    }

    // Create
    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostDto request)
    {
        var author = await _users.GetSingleAsync(request.UserId);
        if (author == null) return BadRequest("User with id " + request.UserId + " does not exist.");

        var post = new Post { Title = request.Title, Body = request.Body, UserId = request.UserId };
        var created = await _posts.AddAsync(post);
        var dto = await ToDtoAsync(created, includeAuthor: true, includeComments: false);
        return Created("/posts/" + dto.Id, (object)dto);
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PostDto>> Update([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        var existing = await _posts.GetSingleAsync(id);
        if (existing == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Title)) existing.Title = request.Title!;
        if (!string.IsNullOrWhiteSpace(request.Body)) existing.Body = request.Body!;
        if (request.UserId.HasValue)
        {
            var user = await _users.GetSingleAsync(request.UserId.Value);
            if (user == null) return BadRequest("User with id " + request.UserId.Value + " does not exist.");
            existing.UserId = request.UserId.Value;
        }

        await _posts.UpdateAsync(existing);
        var dto = await ToDtoAsync(existing, includeAuthor: true, includeComments: false);
        return Ok(dto);
    }

    // Get single with optional includes
    [HttpGet("{id:int}")]
    public async Task<IResult> GetSingle(
        [FromRoute] int id,
        [FromQuery] bool includeAuthor,
        [FromQuery] bool includeComments)
    {
        IQueryable<Post> queryForPost = _posts
            .GetMany()
            .Where(p => p.Id == id)
            .AsQueryable();

        if (includeAuthor)
        {
            queryForPost = queryForPost.Include(p => p.User);
        }

        if (includeComments)
        {
            queryForPost = queryForPost.Include(p => p.Comments);
        }

        PostDto? dto = await queryForPost.Select(post => new PostDto()
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                AuthorUsername = includeAuthor && post.User != null
                    ? post.User.Username
                    : null,
                Comments = includeComments
                    ? post.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Body = c.Body,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        Username = null // Will need to be loaded separately if needed
                    }).ToList()
                    : new List<CommentDto>()
            })
            .FirstOrDefaultAsync();

        return dto == null ? Results.NotFound() : Results.Ok(dto);
    }

    // Get many with filters and optional includes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetMany([FromQuery] GetPostsQuery query)
    {
        var q = _posts.GetMany().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.TitleContains))
        {
            var needle = query.TitleContains.ToUpperInvariant();
            q = q.Where(p => (p.Title ?? string.Empty).ToUpperInvariant().Contains(needle));
        }

        if (query.UserId.HasValue)
            q = q.Where(p => p.UserId == query.UserId.Value);

        if (!string.IsNullOrWhiteSpace(query.AuthorNameContains))
        {
            var needle = query.AuthorNameContains.ToUpperInvariant();
            var allowedUserIds = await _users.GetMany()
                .Where(u => ((u.Name ?? u.Username) ?? string.Empty).ToUpperInvariant().Contains(needle))
                .Select(u => u.Id)
                .ToListAsync();
            var allowedUserIdsSet = allowedUserIds.ToHashSet();
            q = q.Where(p => allowedUserIdsSet.Contains(p.UserId));
        }

        if (query.Skip is > 0) q = q.Skip(query.Skip.Value);
        if (query.Take is > 0) q = q.Take(query.Take.Value);

        // materialize
        var posts = await q.ToListAsync();
        var result = new List<PostDto>(posts.Count);
        foreach (var p in posts)
        {
            result.Add(await ToDtoAsync(p, query.IncludeAuthor, query.IncludeComments));
        }
        return Ok(result);
    }

    // Delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var existing = await _posts.GetSingleAsync(id);
        if (existing == null) return NotFound();
        await _posts.DeleteAsync(id);
        return NoContent();
    }

    // Nested: GET /posts/{postId}/comments
    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost([FromRoute] int postId, [FromQuery] int? userId = null, [FromQuery] string? usernameContains = null)
    {
        var comments = _comments.GetMany().Where(c => c.PostId == postId);
        if (userId.HasValue) comments = comments.Where(c => c.UserId == userId.Value);
        var list = await comments.ToListAsync();

        // username filter and projection
        var users = await _users.GetMany().ToDictionaryAsync(u => u.Id, u => u.Username);
        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            var needle = usernameContains.ToUpperInvariant();
            var allowedUserIds = users.Where(kv => (kv.Value ?? string.Empty).ToUpperInvariant().Contains(needle))
                .Select(kv => kv.Key).ToHashSet();
            list = list.Where(c => allowedUserIds.Contains(c.UserId)).ToList();
        }

        var dtos = list.Select(c => new CommentDto
        {
            Id = c.Id,
            Body = c.Body,
            PostId = c.PostId,
            UserId = c.UserId,
            Username = users.GetValueOrDefault(c.UserId)
        }).ToList();

        return Ok(dtos);
    }

    // Nested: POST /posts/{postId}/comments
    [HttpPost("{postId:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddCommentToPost([FromRoute] int postId, [FromBody] CreateCommentDto request)
    {
        var post = await _posts.GetSingleAsync(postId);
        if (post == null) return NotFound("Post " + postId + " not found.");

        var user = await _users.GetSingleAsync(request.UserId);
        if (user == null) return BadRequest("User with id " + request.UserId + " does not exist.");

        var comment = new Comment { Body = request.Body, PostId = postId, UserId = request.UserId };
        var created = await _comments.AddAsync(comment);
        var dto = new CommentDto
        {
            Id = created.Id,
            Body = created.Body,
            PostId = created.PostId,
            UserId = created.UserId,
            Username = user.Username
        };
        return Created("/comments/" + dto.Id, (object)dto);
    }

    private async Task<PostDto> ToDtoAsync(Post post, bool includeAuthor, bool includeComments)
    {
        string? authorUsername = null;
        IReadOnlyCollection<CommentDto>? commentDtos = null;

        if (includeAuthor)
        {
            var author = await _users.GetSingleAsync(post.UserId);
            authorUsername = author?.Username;
        }

        if (includeComments)
        {
            var list = await _comments.GetMany().Where(c => c.PostId == post.Id).ToListAsync();
            var userLookup = await _users.GetMany().ToDictionaryAsync(u => u.Id, u => u.Username);
            commentDtos = list.Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                PostId = c.PostId,
                UserId = c.UserId,
                Username = userLookup.GetValueOrDefault(c.UserId)
            }).ToList();
        }

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorUsername = authorUsername,
            Comments = commentDtos
        };
    }
}