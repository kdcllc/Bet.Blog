﻿using Core.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Known issues with running inside Docker container.
    /// https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/key-vault/service-to-service-authentication.md
    /// https://rahulpnath.com/blog/authenticating-with-azure-key-vault-using-managed-service-identity/
    /// AzureServicesAuthConnectionString=RunAs=App;AppId=AppId;TenantId=TenantId;AppKey=Secret
    /// </summary>
    public static class AzureVaultKeyBuilder
    {
        private static Dictionary<string, string> _enviroments = new Dictionary<string, string>
        {
            {"Development", "dev" },
            {"Staging", "qa" },
            {"Production", "prod" }
        };

        public static IConfigurationRoot AddAzureKeyVault(
            this IConfigurationBuilder builder,
            IHostingEnvironment hostingEnviroment,
            bool usePrefix = true)
        {
            var config = builder.Build();
            var options = config.Bind<AzureVaultOptions>("AzureVault");

            var prefix = string.Empty;
            if (usePrefix)
            {
                _enviroments.TryGetValue(hostingEnviroment.EnvironmentName, out prefix);
            }

            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                try
                {
                    var policy = Policy
                        .Handle<AzureServiceTokenProviderException>()
                        .WaitAndRetry(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)));

                    var azureServiceTokenProvider = new AzureServiceTokenProvider();

                    Func<KeyVaultClient> kv = () => new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                    var keyVaultClient = policy.Execute(kv);

                    if (!string.IsNullOrEmpty(prefix))
                    {
                        builder.AddAzureKeyVault(options.BaseUrl, keyVaultClient, new PrefixKeyVaultSecretManager(prefix));
                    }
                    else
                    {
                        builder.AddAzureKeyVault(options.BaseUrl, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }

                   return builder.Build();
                }
                catch (AzureServiceTokenProviderException)
                {
                    var list = builder.Sources.ToList();
                    var found = list.FirstOrDefault(x => x.GetType().FullName.Contains("AzureKeyVaultConfigurationSource"));
                    if (found != null)
                    {
                        builder.Sources.Remove(found);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(options.ClientId)
                && !string.IsNullOrWhiteSpace(options.ClientSecret))
            {
                var secretBytes = Convert.FromBase64String(options.ClientSecret);
                var secret = System.Text.Encoding.ASCII.GetString(secretBytes);

                if (!string.IsNullOrEmpty(prefix))
                {
                    builder.AddAzureKeyVault(options.BaseUrl, options.ClientId, secret, new PrefixKeyVaultSecretManager(prefix));
                }
                else
                {
                    builder.AddAzureKeyVault(options.BaseUrl, options.ClientId, secret, new DefaultKeyVaultSecretManager());
                }
            }

            return builder.Build();
        }
    }
}
