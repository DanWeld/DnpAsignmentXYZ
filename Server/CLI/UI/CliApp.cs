using Entities;
using InMemoryRepositories;
using RepositoryContracts;
namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _postRepository = postRepository;
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Welcome to the CLI App!");
        User? loggedInUser = null;

        while (true)
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Create User");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var loginView = new ManageUsers.LoginUserView(_userRepository);
                        loggedInUser = await loginView.ShowAsync();
                        break;
                    case "2":
                        var createUserView = new ManageUsers.CreateUserView(_userRepository);
                        await createUserView.ShowAsync();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Welcome, {loggedInUser.Username}!");
                Console.WriteLine("1. Create Post");
                Console.WriteLine("2. Logout");
                Console.Write("Choose an option: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        var createPostView = new ManagePosts.CreatePostView(_postRepository, loggedInUser);
                        await createPostView.ShowAsync();
                        break;
                    case "2":
                        loggedInUser = null;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
    }
}

