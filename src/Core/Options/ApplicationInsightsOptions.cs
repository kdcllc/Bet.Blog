﻿using System.ComponentModel.DataAnnotations;

namespace Core.Options
{
    public class ApplicationInsightsOptions
    {
        /// <summary>
        /// Azure Instrumentation Key. Guid Type.
        /// </summary>
        [RegularExpression(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", ErrorMessage = "Must be valid Guid Id")]
        public string InstrumentationKey { get; set; }

        /// <summary>
        /// Enables Event tracing.
        /// </summary>
        public bool EnableEvents { get; set; }

        /// <summary>
        /// Enables Traces with Azure AppInsights.
        /// </summary>
        public bool EnableTraces { get; set; }
    }
}
