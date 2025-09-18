namespace CLI.UI.ManageUsers;

public class ListUsersView
{

    public void Render()
    {
        Console.Clear();
        Console.WriteLine("...");
        Console.WriteLine("1. User1");
        Console.WriteLine("2. User2");
        Console.WriteLine("3. User3");
        Console.WriteLine("...");
        Console.WriteLine("Press any key ...");
        Console.ReadKey();
    }
}