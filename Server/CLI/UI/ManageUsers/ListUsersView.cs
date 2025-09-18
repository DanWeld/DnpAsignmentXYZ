namespace CLI.UI.ManageUsers;

public class ListUsersView
{

    public void DisplayUsers(List<Entities.User> users)
    {
        Console.WriteLine("List of Users:");
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Password: {user.Password}");
        }
        
    }
}