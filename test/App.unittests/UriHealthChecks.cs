using Microsoft.AspNetCore.Hosting;
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
        public void Check()
        {
            var host = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                    .AddUriHealthCheck("Successful", builder =>
                    {
                        builder.Add(options =>
                        {
                            options.AddUri("http://localhost/api/HttpStat/200").UseExpectedHttpCode(200);
                        });
                    });
                });
        }
    }
}
