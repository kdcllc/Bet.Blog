﻿using System.IO.Compression;
using System.Net;

using Bet.Blog.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Bet.Blog.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(p => p.AddSerilog(dispose: false));

            var healthCheckBuilder = services.AddHealthChecks();

            healthCheckBuilder.AddMemoryHealthCheck(thresholdInBytes: 1024L * 1024L);

            healthCheckBuilder.AddUriHealthCheck("Success Codes", check =>
            {
                check.Add(option =>
                {
                    option.AddUri("https://httpstat.us/200")
                           .UseExpectedHttpCode(HttpStatusCode.OK);
                });

                check.Add(option =>
                {
                    option.AddUri("https://httpstat.us/203")
                           .UseExpectedHttpCode(HttpStatusCode.NonAuthoritativeInformation);
                });
            });

            // Enable GZip and Brotli compression.
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddOptions<AppSettings>()
                .Validate(x =>
                {
                    return true;
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<BloggingContext>(options =>
            {
                var provider = Configuration.GetValue<string>("DatabaseProvider");
                if (provider == "Sqlite")
                {
                    // used for k8s PVC mapping, otherwise is created in the root
                    var dbPath = Configuration.GetValue<string>("DatabasePath");

                    var connectionString = $"Filename={dbPath}blog.db";

                    options.UseSqlite(
                        connectionString,
                        b => b.MigrationsAssembly("Bet.Blog.Data.Sqlite"));
                }

                if (provider == "SqlServer")
                {
                    options.UseSqlServer(
                        Configuration.GetConnectionString("SqlServerConnection"),
                        b => b.MigrationsAssembly("Bet.Blog.Data.SqlServer"));
                }
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.User.AllowedUserNameCharacters = null;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<BloggingContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseResponseCompression();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapLivenessHealthCheck();
                endpoints.MapHealthyHealthCheck();
            });
        }
    }
}