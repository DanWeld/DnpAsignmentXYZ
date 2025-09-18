namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    public void DisplayPost(int postId, string title, string body, string author)
    {
        Console.Clear();
        Console.WriteLine("Post Details");
        Console.WriteLine("...");
        Console.WriteLine($"ID: {postId}");
        Console.WriteLine($"Title: {title}");
        Console.WriteLine($"Body: {body}");
        Console.WriteLine($"Author: {author}");
        Console.WriteLine("...");
        Console.WriteLine("Press any key ...");
        Console.ReadKey();
    }


}