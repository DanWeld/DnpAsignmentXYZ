using BlazorApp.Client.Pages;
using BlazorApp.Components;
using BlazorApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp.Client.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Actual authentication happens client-side in WebAssembly
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// Add authorization - required for AuthorizeView components and middleware
builder.Services.AddAuthorization();

// Register HttpClient for server-side
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7005/") });

// Register services for dependency injection
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IPostService, HttpPostService>();
builder.Services.AddScoped<ICommentService, HttpCommentService>();

// Register authentication state provider for WebAssembly
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

app.Run();