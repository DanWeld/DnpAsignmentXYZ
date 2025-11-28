// Add a type alias to disambiguate System.AppContext vs EfcRepositories.AppContext
using AppContext = EfcRepositories.AppContext;
using EfcRepositories;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add authorization services (for AuthController)
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register DbContext
builder.Services.AddDbContext<AppContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Register repositories
builder.Services.AddScoped<IPostRepository, EfcPostRepository>();
builder.Services.AddScoped<ICommentRepository, EfcCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfcUserRepository>();

var app = builder.Build();

// Ensure database is created (use EnsureCreated when migrations are not present)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppContext>();
    try
    {
        // Apply migrations if present (preferred when migrations exist)
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to create or migrate database: " + ex);
    }
}

// Use CORS
app.UseCors("AllowBlazorApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ensure authorization middleware is registered (requires AddAuthorization above)
app.UseAuthorization();

app.MapControllers();

app.Run();
