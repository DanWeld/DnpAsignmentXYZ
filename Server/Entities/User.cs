namespace Entities;

public class User
{
    // Constructor for EFC
    public User() { }

    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}