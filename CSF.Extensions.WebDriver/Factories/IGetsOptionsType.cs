using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// An object which can get the concrete type of some WebDriver Options, indicated by the configuration.
    /// </summary>
    public interface IGetsOptionsType
    {
        /// <summary>
        /// Validates and gets the <see cref="Type"/> of the implementation of <see cref="DriverOptions"/> implementation indicated by the configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that it is valid for the options type to be <see langword="null"/> if <see cref="WebDriverCreationOptions.DriverFactoryType"/> is specified.
        /// In that scenario, the options type is unused, but it still indicates a valid configuration.
        /// </para>
        /// </remarks>
        /// <param name="options">The options, as they have been parsed so far</param>
        /// <param name="configuration">The configuration section</param>
        /// <param name="driverType">The type of the Web Driver, as has already been determined by
        /// <see cref="IGetsDriverType.TryGetDriverType(WebDriverCreationOptions, IConfigurationSection, out Type)"/>.</param>
        /// <param name="optionsType">If this method returns <see langword="true"/> then this is a <see cref="Type"/> of the driver options, otherwise
        /// this value is undefined and must be ignored.</param>
        /// <returns><see langword="true"/> if the driver type information is valid; <see langword="false"/> if not</returns>
        bool TryGetOptionsType(WebDriverCreationOptions options, IConfigurationSection configuration, Type driverType, out Type optionsType);
    }
}

