﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Hosting
{
    public class OptionsValidationStartupFilter : IStartupFilter
    {
        private IList<(Type type, string sectionName)> _optionsTypes;

        /// <summary>
        /// The type and configuration name of the options to validate.
        /// </summary>
        public IList<(Type type, string sectionName)> OptionsTypes => _optionsTypes ?? (_optionsTypes = new List<(Type type, string sectionName)>());

        /// <inheritdoc/>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                if (_optionsTypes != null)
                {
                    foreach (var (type, sectionName) in _optionsTypes)
                    {
                        var options = app.ApplicationServices.GetService(typeof(IOptions<>).MakeGenericType(type));
                        try
                        {
                            if (options != null)
                            {
                                // Retrieve the value to trigger validation
                                var optionsValue = ((IOptions<object>)options).Value;
                            }
                        }
                        catch (OptionsValidationException ex)
                        {
                            throw new Core.Exceptions.OptionsValidationException(ex.Failures, (type, sectionName));
                        }
                    }
                }

                next(app);
            };
        }
    }
}
