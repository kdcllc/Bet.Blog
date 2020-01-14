using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

namespace Bet.Blog.WebApp
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
                        {
                            var envName = hostingContext.HostingEnvironment.EnvironmentName;
                            var configuration = configBuilder.AddAzureKeyVault(
                                hostingEnviromentName: envName,
                                usePrefix: true,
                                reloadInterval: TimeSpan.FromSeconds(20));

                            // if (hostingContext.HostingEnvironment.IsDevelopment())
                            // {
                                configuration.DebugConfigurations();
                            //}
                        });

                        webBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            var applicationName = $"BetBlog-{hostingContext.HostingEnvironment.EnvironmentName}";
                            loggerConfiguration
                                    .ReadFrom.Configuration(hostingContext.Configuration)
                                    .Enrich.FromLogContext()
                                    .WriteTo.Console()
                                    .AddApplicationInsights(hostingContext.Configuration)
                                    .AddAzureLogAnalytics(hostingContext.Configuration, applicationName: applicationName);
                        });

                        webBuilder.UseStartup<Startup>()
                        .ConfigureKestrel(a => a.AddServerHeader = false);
                    });
        }
    }
}
