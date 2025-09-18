namespace CLI.UI.ManageUsers;

using RepositoryContracts;
using Entities;

public class CreateUserView
{
    private readonly IUserRepository _userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Enter user name: ");
        var name = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = Console.ReadLine();
        await _userRepository.AddAsync(new User { Username = name, Password = password, Id = 0 });
        Console.WriteLine("User created.");
    }
}