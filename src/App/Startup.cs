using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DabarBlog.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Serilog;
using Infrastructure.HealthChecks.UriCheck;
using System.Net;

namespace DabarBlog
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

            healthCheckBuilder.AddUriHealthCheck("Success Codes", check=>
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

            //healthCheckBuilder.AddUriHealthCheck("Check Name 2", check =>
            //{
            //    check.Add(registration => { registration.Name = "Check2-Uri1"; });
            //    check.Add(registration => { registration.Name = "Check2-Uri2"; });
            //});

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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                // Exclude all checks and return a 200-Ok. Default registered health check is self.
                Predicate = (p) => false
            });

            app.UseHealthChecks("/healthy", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckBuilderExtensions.WriteResponse
            });

            app.UseResponseCompression();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
