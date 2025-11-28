namespace Entities;

public class Post
{
    // Constructor for EFC
    public Post() { }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}