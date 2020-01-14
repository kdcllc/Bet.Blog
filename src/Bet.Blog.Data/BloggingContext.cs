using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bet.Blog.Data
{
    public class BloggingContext : IdentityDbContext<AppUser>
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
