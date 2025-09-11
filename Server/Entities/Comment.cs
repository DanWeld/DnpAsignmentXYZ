namespace Entities;

public class Comment
{
    
    public  int Id { get; set; }
    public string Body { get; set; } 
    public int PostId { get; set; } // Foreign key to Post
    public int UserId { get; set; } // Foreign key to User
    
}