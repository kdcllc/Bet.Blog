using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data.SqlServer
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
                "Server=(localdb)\\mssqllocaldb;Database=DabarBlog;Trusted_Connection=True;MultipleActiveResultSets=true",
                b => b.MigrationsAssembly("Infrastructure.Data.SqlServer"));

            return new BloggingContext(optionsBuilder.Options);
        }
    }
}
