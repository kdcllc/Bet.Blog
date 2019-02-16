﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.HealthChecks.UriCheck
{
    public class UriHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<UriHealthCheckOptions> _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private Dictionary<string, object> _data = new Dictionary<string, object>();
        private bool _IsHealthy = true;

        public UriHealthCheck(
            IOptionsMonitor<UriHealthCheckOptions> options,
            IHttpClientFactory httpClientFactory)
        {
            _options = options;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var index = 0;
            try
            {
                var checkName = context.Registration.Name;

                var options = _options.Get(checkName);

                foreach (var option in options.UriOptions)
                {
                    var httpClient = _httpClientFactory.CreateClient(checkName);

                    var requestMessage = new HttpRequestMessage(option.HttpMethod, option.Uri);

                    foreach (var header in option.Headers)
                    {
                        requestMessage.Headers.Add(header.Name, header.Value);
                    }

                    using (var timeoutSource = new CancellationTokenSource(option.Timeout))
                    using (var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken))
                    {
                        var response = await httpClient.SendAsync(requestMessage, linkedSource.Token);

                        if (!((int)response.StatusCode >= option.ExpectedHttpCodes.Min && (int)response.StatusCode <= option.ExpectedHttpCodes.Max))
                        {
                            _IsHealthy = false;

                            var errorMessage = $"Discover endpoint #{index} is not responding with code in {option.ExpectedHttpCodes.Min}...{option.ExpectedHttpCodes.Max} range, the current status is {response.StatusCode}.";

                            _data.Add(option.Uri.ToString(), errorMessage);
                        }
                        else
                        {
                            var message = $"Discovered endpoint #{index} is responding with {response.StatusCode}.";
                            _data.Add(option.Uri.ToString(), message);
                        }
                        ++index;
                    }
                }

                var status = _IsHealthy ? HealthStatus.Healthy : context.Registration.FailureStatus;

                return new HealthCheckResult(
                    status,
                    description: $"Reports degraded status if one of {index} failed",
                    exception: null,
                    data: _data);
            }
            catch (Exception ex)
            {
                //TODO: not expose all of the exception details for security reasons.
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
