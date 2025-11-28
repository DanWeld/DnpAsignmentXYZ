namespace Entities;

public class Comment
{
    // Constructor for EFC
    public Comment() { }

    public int Id { get; set; }
    public string Body { get; set; }
    public int PostId { get; set; } // Foreign key to Post
    public int UserId { get; set; } // Foreign key to User

    // Navigation properties
    public Post? Post { get; set; }
    public User? User { get; set; }
}