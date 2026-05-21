using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Decorator for <see cref="ICreatesDriverOptions"/> which binds the configuration to the created options.
    /// </summary>
    public class ConfigBindingDriverOptionsFactoryDecorator : ICreatesDriverOptions
    {
        readonly ICreatesDriverOptions wrapped;

        /// <inheritdoc/>
        public DriverOptions CreateOptions(Type optionsType, IConfigurationSection config)
        {
            var options = wrapped.CreateOptions(optionsType, config);
            config.Bind("Options", options);
            return options;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ConfigBindingDriverOptionsFactoryDecorator"/>.
        /// </summary>
        /// <param name="wrapped">The wrapped service</param>
        /// <exception cref="ArgumentNullException">If <paramref name="wrapped"/> is <see langword="null"/>.</exception>
        public ConfigBindingDriverOptionsFactoryDecorator(ICreatesDriverOptions wrapped)
        {
            this.wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
        }
    }
}