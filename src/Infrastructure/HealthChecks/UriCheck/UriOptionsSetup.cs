﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Infrastructure.HealthChecks.UriCheck
{
    /// <summary>
    /// The class to provide configuration for the healthcheck request.
    /// </summary>
    public class UriOptionsSetup : IUriOptionsSetup
    {
        private readonly List<(string Name, string Value)> _headers = new List<(string Name, string Value)>();
        internal IEnumerable<(string Name, string Value)> Headers => _headers;

        public HttpMethod HttpMethod { get; private set; }

        public TimeSpan Timeout { get; private set; }

        public (int Min, int Max) ExpectedHttpCodes { get; private set; }

        public Uri Uri { get; private set; }

        /// <summary>
        /// Default values for
        /// </summary>
        /// <param name="uri"></param>
        public UriOptionsSetup(Uri uri = default)
        {
            Uri = uri;

            // success codes
            ExpectedHttpCodes = (200, 226);

            HttpMethod = HttpMethod.Get;

            Timeout = TimeSpan.FromSeconds(10);
        }

        /// <inheritdoc/>
        public IUriOptionsSetup AddUri(Uri uri)
        {
            Uri = uri;
            return this;
        }

        /// <inheritdoc/>
        public IUriOptionsSetup AddUri(string uri)
        {
            Uri = new Uri(uri);

            return this;
        }

        /// <inheritdoc/>
        public IUriOptionsSetup UseTimeout(TimeSpan timeout)
        {
            Timeout = timeout;
            return this;
        }

        /// <inheritdoc/>
        public IUriOptionsSetup UseHttpMethod(HttpMethod httpMethod)
        {
            HttpMethod = httpMethod;

            return this;
        }

        /// <inheritdoc/>
        public IUriOptionsSetup UseExpectHttpCodes(int min, int max)
        {
            ExpectedHttpCodes = (min, max);

            return this;
        }

        /// <inheritdoc/>
        public IUriOptionsSetup UseExpectedHttpCode(HttpStatusCode statusCode)
        {
            var code = (int)statusCode;

            ExpectedHttpCodes = (code,code);

            return this;
        }

        /// <inheritdoc/>
        public IUriOptionsSetup UseExpectedHttpCode(int statusCode)
        {
            ExpectedHttpCodes = (statusCode, statusCode);

            return this;
        }

        public IUriOptionsSetup AddCustomHeader(string name, string value)
        {
            _headers.Add((name, value));

            return this;
        }
    }
}
