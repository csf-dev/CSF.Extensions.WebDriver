using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Implementation of <see cref="ICreatesDriverOptions"/> which uses <see cref="Activator.CreateInstance(Type)"/> to create the options object.
    /// </summary>
    public class ActivatorDriverOptionsFactory : ICreatesDriverOptions
    {
        /// <inheritdoc/>
        public DriverOptions CreateOptions(Type optionsType, IConfigurationSection config)
            => (DriverOptions) Activator.CreateInstance(optionsType);
    }
}