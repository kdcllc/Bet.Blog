using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data.Sqlite
{
    /// <summary>
    /// dotnet ef migrations add InitialCreate --context BloggingContext --output-dir Migrations/.
    /// </summary>
    internal class BloggingContextFactory : IDesignTimeDbContextFactory<BloggingContext>
    {
        public BloggingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();

            optionsBuilder.UseSqlite("Data Source=blog.db", b => b.MigrationsAssembly("Infrastructure.Data.Sqlite"));

            return new BloggingContext(optionsBuilder.Options);
        }
    }
}
