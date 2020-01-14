using Bet.Blog.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbContextServiceExtensions
    {
        public static IServiceCollection AddDbContextAndIdentity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<BloggingContext>(options =>
            {
                var provider = configuration.GetValue<string>("DatabaseProvider");
                if (provider == "Sqlite")
                {
                    // used for k8s PVC mapping, otherwise is created in the root
                    var dbPath = configuration.GetValue<string>("DatabasePath");

                    var connectionString = $"Filename={dbPath}blog.db";

                    options.UseSqlite(
                        connectionString,
                        b => b.MigrationsAssembly("Bet.Blog.Data.Sqlite"));
                }

                if (provider == "SqlServer")
                {
                    options.UseSqlServer(
                        configuration.GetConnectionString("SqlServerConnection"),
                        b => b.MigrationsAssembly("Bet.Blog.Data.SqlServer"));
                }
            });

            services.AddDefaultIdentity<AppUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = null;
            })
            .AddEntityFrameworkStores<BloggingContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
