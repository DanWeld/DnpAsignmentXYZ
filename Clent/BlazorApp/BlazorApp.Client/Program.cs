using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register HttpClient - Use the hosting app's base address by default for Blazor WebAssembly
// Then configure a named HttpClient for the API
builder.Services.AddScoped(sp => 
{
    // Create HttpClient that points to the WebAPI
    var httpClient = new HttpClient 
    { 
        BaseAddress = new Uri("https://localhost:7005/") 
    };
    return httpClient;
});

// Register services
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IPostService, HttpPostService>();
builder.Services.AddScoped<ICommentService, HttpCommentService>();

await builder.Build().RunAsync();