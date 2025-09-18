using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
 private readonly IPostRepository postRepository;
    public CreatePostView(IPostRepository postRepository, User loggedInUser)
    {
        this.postRepository = postRepository;

    }
    public async Task DisplayAsync()
    {
        Console.WriteLine("Create a New Post");
        Console.Write("Enter Title: ");
        var title = Console.ReadLine();
        Console.Write("Enter Body: ");
        var body = Console.ReadLine();
        Console.Write("Enter UserId: ");
        var userIdInput = Console.ReadLine();
        if (!int.TryParse(userIdInput, out var userId))
        {
            Console.WriteLine("Invalid UserId. It must be a number.");
            return;
        }

        var newPost = new Entities.Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        var createdPost = await postRepository.AddAsync(newPost);
        Console.WriteLine($"Post created successfully with ID: {createdPost.Id}");
    }


    public async Task ShowAsync()
    {
        await DisplayAsync();
        
    }
}

