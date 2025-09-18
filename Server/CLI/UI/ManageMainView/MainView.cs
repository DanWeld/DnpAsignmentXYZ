namespace CLI.UI.ManageMainView;

using RepositoryContracts;
using Entities;

public class MainView
{
    private readonly IUserRepository _users;
    private readonly IPostRepository _posts;
    private readonly ICommentRepository _comments;
    private readonly User _user;

    public MainView(IUserRepository users, IPostRepository posts, ICommentRepository comments, User user)
    { _users = users; _posts = posts; _comments = comments; _user = user; }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"Welcome, {_user.Username} (User {_user.Id})");
            Console.WriteLine("1. Create Post");
            Console.WriteLine("2. Add Comment");
            Console.WriteLine("3. Posts Overview");
            Console.WriteLine("4. View Post");
            Console.WriteLine("0. Logout");
            Console.Write("Select: ");
            var c = Console.ReadLine();
            if (c == "1") await CreatePostAsync();
            else if (c == "2") await AddCommentAsync();
            else if (c == "3") ShowPostsOverview();
            else if (c == "4") ViewSinglePost();
            else if (c == "0") return; else Console.WriteLine("Invalid");
        }
    }

    private async Task CreatePostAsync()
    {
        Console.Write("Title: "); var title = Console.ReadLine();
        Console.Write("Body: "); var body = Console.ReadLine();
        await _posts.AddAsync(new Post { Title = title, Body = body, UserId = _user.Id });
        Console.WriteLine("Post created.");
    }

    private async Task AddCommentAsync()
    {
        Console.Write("Post Id: ");
        if (!int.TryParse(Console.ReadLine(), out var pid)) { Console.WriteLine("Bad id"); return; }
        if (!_posts.GetMany().Any(p => p.Id == pid)) { Console.WriteLine("Post not found"); return; }
        Console.Write("Comment: "); var body = Console.ReadLine();
        await _comments.AddAsync(new Comment { Body = body, PostId = pid, UserId = _user.Id });
        Console.WriteLine("Comment added.");
    }

    private void ShowPostsOverview()
    {
        var list = _posts.GetMany().OrderBy(p => p.Id).ToList();
        if (!list.Any()) { Console.WriteLine("No posts."); return; }
        foreach (var p in list) Console.WriteLine($"{p.Id}: {p.Title}");
    }

    private void ViewSinglePost()
    {
        Console.Write("Post Id: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Bad id"); return; }
        Post post; try { post = _posts.GetSingleAsync(id).Result; } catch { Console.WriteLine("Not found"); return; }
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        var comm = _comments.GetMany().Where(c => c.PostId == post.Id).OrderBy(c => c.Id).ToList();
        Console.WriteLine("Comments:");
        if (!comm.Any()) Console.WriteLine("(none)"); else foreach (var c in comm) Console.WriteLine($"{c.Id} (U{c.UserId}): {c.Body}");
    }
}
