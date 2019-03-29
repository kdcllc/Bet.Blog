using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DabarBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
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
    }
}
