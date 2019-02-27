using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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

                })
                .Configure(app=>
                {

                });

         }
    }
}
