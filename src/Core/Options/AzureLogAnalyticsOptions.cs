﻿using System.ComponentModel.DataAnnotations;

namespace Core.Options
{
    public class AzureLogAnalyticsOptions
    {
        /// <summary>
        /// Azure Workspace Id. Guid Id type.
        /// </summary>
        [RegularExpression(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", ErrorMessage = "Must be valid Guid Id")]
        public string WorkspaceId { get; set; }

        /// <summary>
        /// Base64 encrypted value.
        /// </summary>
        public string AuthenticationId { get; set; }
    }
}
