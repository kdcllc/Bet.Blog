using System.Collections.Generic;

namespace Infrastructure.HealthChecks.UriCheck
{
    public class UriHealthCheckOptions
    {
        public ICollection<UriOptionsSetup> UriOptions { get; } = new List<UriOptionsSetup>();
    }
}
