using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationBinderExtensions
    {
        /// <summary>
        /// Attempts to bind the given object instance to the configuration section
        /// specified by the key by matching property names against configuration keys recursively.
        /// This method binds the instance of the object and
        /// the validation is applied <see cref="Microsoft.Extensions.Options.DataAnnotationValidateOptions{TOptions}"/>.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="key">The key of the configuration section to bind.</param>
        /// <param name="instance">The object to bind.</param>
        public static void Bind<TOptions>(
            this IConfiguration configuration,
            string key,
            object instance) where TOptions : class, new()
        {
            configuration.Bind(key, instance);

            ValidateDataAnnotation<TOptions>(instance, key);
        }

        /// <summary>
        /// Attempts to bind the given object instance to the configuration section
        /// specified by the key by matching property names against configuration keys recursively.
        /// This method binds the instance of the object and
        /// the validation delegate is applied.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="key">The key of the configuration section to bind.</param>
        /// <param name="instance">The object to bind.</param>
        /// <param name="validation">The validation delegate.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void Bind<TOptions>(
            this IConfiguration configuration,
            string key,
            object instance,
            Func<TOptions, bool> validation,
            string failureMessage) where TOptions : class, new()
        {
            configuration.Bind(key, instance);

            var section = (IConfigurationSection)configuration;

            Validate(instance, validation, failureMessage, section.Path);
        }

        /// <summary>
        /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
        /// This method binds the instance of the object and
        /// the validation is applied <see cref="Microsoft.Extensions.Options.DataAnnotationValidateOptions{TOptions}"/>.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="instance">The object to bind.</param>
        public static void Bind<TOptions>(
            this IConfiguration configuration,
            object instance) where TOptions : class, new()
        {
            configuration.Bind(instance);

            var section = (IConfigurationSection)configuration;

            ValidateDataAnnotation<TOptions>(instance, section.Path);
        }

        /// <summary>
        /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
        /// This method binds the instance of the object and
        /// the validation delegate is applied.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="instance">The object to bind.</param>
        /// <param name="validation">The validation delegate.</param>
        /// <param name="failureMessage">The failure message.</param>
        public static void Bind<TOptions>(
            this IConfiguration configuration,
            object instance,
            Func<TOptions, bool> validation,
            string failureMessage) where TOptions : class, new()
        {
            configuration.Bind(instance);

            var section = (IConfigurationSection)configuration;

            Validate(instance, validation, failureMessage, section.Path);
        }

        /// <summary>
        /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
        /// This method binds the instance of the object and
        /// the validation is applied <see cref="Microsoft.Extensions.Options.DataAnnotationValidateOptions{TOptions}"/>.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="configuration">The configuration instance to bind.</param>
        /// <param name="instance">The object to bind.</param>
        /// <param name="configureOptions">Configures the binder options.</param>
        public static void Bind<TOptions>(
            this IConfiguration configuration,
            object instance,
            Action<BinderOptions> configureOptions) where TOptions : class, new()
        {
            configuration.Bind(instance, configureOptions);

            var section = (IConfigurationSection)configuration;

            ValidateDataAnnotation<TOptions>(instance, section.Path);
        }

        /// <summary>
        /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
        /// This method binds the instance of the object and
        /// the validation delegate is applied.
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="instance"></param>
        /// <param name="configureOptions"></param>
        /// <param name="validation"></param>
        /// <param name="failureMessage"></param>
        public static void Bind<TOptions>(
            this IConfiguration configuration,
            object instance,
            Action<BinderOptions> configureOptions,
            Func<TOptions, bool> validation,
            string failureMessage) where TOptions : class, new()
        {
            configuration.Bind(instance, configureOptions);

            var section = (IConfigurationSection)configuration;

            Validate(instance, validation, failureMessage, section.Path);
        }

        private static void Validate<TOptions>(
            object instance,
            Func<TOptions, bool> validation,
            string failureMessage,
            string sectionName) where TOptions : class, new()
        {
            var copyInstance = (TOptions)instance;

            if (!validation(copyInstance))
            {
                ThrowValidateException<TOptions>(ValidateOptionsResult.Fail(failureMessage), sectionName);
            }
        }

        private static void ValidateDataAnnotation<TOptions>(
            object instance,
            string sectionName) where TOptions : class, new()
        {
            var copyInstance = (TOptions)instance;

            var name = typeof(TOptions).Name;

            var validator = new DataAnnotationValidateOptions<TOptions>(name);

            var result = validator.Validate(name, copyInstance);

            if (result.Failed)
            {
                ThrowValidateException<TOptions>(result, sectionName);
            }
        }

        private static void ThrowValidateException<TOptions>(
            ValidateOptionsResult result,
            string sectionName)
            where TOptions : class, new()
        {
            throw new Core.Exceptions.OptionsValidationException(result.FailureMessage, (typeof(TOptions), sectionName));
        }
    }
}
