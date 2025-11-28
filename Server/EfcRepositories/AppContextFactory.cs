using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace EfcRepositories;

public class AppContextFactory : IDesignTimeDbContextFactory<AppContext>
{
    public AppContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppContext>();
        
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "WebAPI"));
        var dbPath = Path.Combine(basePath, "app.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new AppContext(optionsBuilder.Options);
    }
}
