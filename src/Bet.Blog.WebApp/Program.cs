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
                            var configuration = configBuilder.AddAzureKeyVault(hostingEnviromentName: envName, usePrefix: true);
                            configuration.DebugConfigurationsWithSerilog();
                        })
                        .UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                            .ReadFrom.Configuration(hostingContext.Configuration)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .AddApplicationInsights(hostingContext.Configuration)
                            .AddAzureLogAnalytics(hostingContext.Configuration);
                        })
                        .UseStartup<Startup>()
                        .ConfigureKestrel(a => a.AddServerHeader = false);
                    });
        }
    }
}
