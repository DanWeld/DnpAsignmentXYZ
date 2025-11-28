using System.Net.Http.Json;
using System.Text.Json;

var client = new HttpClient { BaseAddress = new Uri("http://localhost:7005") };

async Task PostUser(string username, string name)
{
    var create = new { username = username, password = "pass", name = name };
    var res = await client.PostAsJsonAsync("/users", create);
    Console.WriteLine($"POST /users {username} => {res.StatusCode}");
    var body = await res.Content.ReadAsStringAsync();
    Console.WriteLine(body);
}

async Task PostPost(int userId)
{
    var create = new { title = "Hello from Dani", body = "This is a test post", userId = userId };
    var res = await client.PostAsJsonAsync("/posts", create);
    Console.WriteLine($"POST /posts user {userId} => {res.StatusCode}");
    Console.WriteLine(await res.Content.ReadAsStringAsync());
}

async Task Get(string path)
{
    var res = await client.GetAsync(path);
    Console.WriteLine($"GET {path} => {res.StatusCode}");
    Console.WriteLine(await res.Content.ReadAsStringAsync());
}

await PostUser("dani","Dani");
await PostUser("dan","Dan");
await PostPost(1);
await Get("/posts/1?includeAuthor=true&includeComments=true");
await Get("/users");
await Get("/posts");

