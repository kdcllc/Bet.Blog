using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bet.Blog.Data.SqlServer
{
    /// <summary>
    /// dotnet ef migrations add InitialCreate --context BloggingContext --output-dir Migrations/.
    /// </summary>
    internal class BloggingContextFactory : IDesignTimeDbContextFactory<BloggingContext>
    {
        public BloggingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();

            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=BetBlog;Trusted_Connection=True;MultipleActiveResultSets=true",
                b => b.MigrationsAssembly("Bet.Blog.Data.SqlServer"));

            return new BloggingContext(optionsBuilder.Options);
        }
    }
}
