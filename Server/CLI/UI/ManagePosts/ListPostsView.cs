namespace CLI.UI.ManagePosts;

public class ListPostsView
{

    public void DisplayPosts(List<Entities.Post> posts)
    {
        Console.WriteLine("List of Posts:");
        foreach (var post in posts)
        {
            Console.WriteLine($"ID: {post.Id}, Title: {post.Title}, Body: {post.Body}, UserId: {post.UserId}");
        }
        
        
        
    }
    
    

}