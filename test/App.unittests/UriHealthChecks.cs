using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace App.unittests
{
    public class UriHealthChecks
    {

        [Fact]
        public async Task Check()
        {
            var statusCode = 200;

            var hostBuilder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                    .AddUriHealthCheck("Successful", builder =>
                    {
                        builder.Add(options =>
                        {
                            options.AddUri($"http://localhost/api/HttpStat/${statusCode}").UseExpectedHttpCode(statusCode);
                        });
                    });
                })
                .Configure(app=>
                {
                    app.UseHealthChecks("/healthy", new HealthCheckOptions
                    {
                        ResponseWriter = HealthCheckBuilderExtensions.WriteResponse
                    });
                });

            var client = new TestServer(hostBuilder).CreateClient();

            var result = await client.GetAsync("/healthy");

            Assert.Equal(statusCode, (int)result.StatusCode);
        }
    }
}
