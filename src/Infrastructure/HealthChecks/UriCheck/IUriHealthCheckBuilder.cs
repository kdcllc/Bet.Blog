using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure.HealthChecks.UriCheck
{
    public interface IUriHealthCheckBuilder
    {
        /// <summary>
        /// Name of the Uris Check Group.
        /// </summary>
        string CheckName { get;}

        IUriHealthCheckBuilder Add(Action<UriOptionsSetup> action);

        IUriHealthCheckBuilder Add(UriOptionsSetup optionsSetup);

        IServiceCollection Services { get; }
    }
}
