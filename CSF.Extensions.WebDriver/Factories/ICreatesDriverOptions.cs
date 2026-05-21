using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// An object which creates and returns a new object which derives from <see cref="DriverOptions"/>.
    /// </summary>
    public interface ICreatesDriverOptions
    {
        /// <summary>
        /// Creates and returns a new driver options instance.
        /// </summary>
        /// <param name="optionsType">The desired type of the options object</param>
        /// <param name="config">The CSF.Extensions.WebDriver configuration</param>
        /// <returns>A new driver options instance</returns>
        DriverOptions CreateOptions(Type optionsType, IConfigurationSection config);
    }
}