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
                    var config = configBuilder.Build();

                    var envName = hostingContext.HostingEnvironment.EnvironmentName;

                    config = configBuilder.AddAzureKeyVault(hostingEnviromentName:envName,usePrefix:true);

                    config.DebugConfigurationsWithSerilog();
                })
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                          .ReadFrom.Configuration(hostingContext.Configuration)
                          .Enrich.FromLogContext()
                          .WriteTo.Console()
                          .AddApplicationInsights(hostingContext.Configuration)
                          .AddAzureLogAnalytics(hostingContext.Configuration);
                })
                .ConfigureKestrel(a => a.AddServerHeader = false);
    }
}
