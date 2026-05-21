using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// An object which can get the concrete type of a WebDriver, indicated by the configuration.
    /// </summary>
    public interface IGetsDriverType
    {
        /// <summary>
        /// Validates and gets the <see cref="Type"/> of the implementation of <see cref="IWebDriver"/> implementation indicated by the configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that it is valid for the driver type to be <see langword="null"/> if <see cref="WebDriverCreationOptions.DriverFactoryType"/> is specified.
        /// In that scenario, the driver type is unused, but it still indicates a valid configuration.
        /// </para>
        /// </remarks>
        /// <param name="options">The options, as they have been parsed so far</param>
        /// <param name="configuration">The configuration section</param>
        /// <param name="driverType">If this method returns <see langword="true"/> then this is a <see cref="Type"/> of the web driver, otherwise
        /// this value is undefined and must be ignored.</param>
        /// <returns><see langword="true"/> if the driver type information is valid; <see langword="false"/> if not</returns>
        bool TryGetDriverType(WebDriverCreationOptions options, IConfigurationSection configuration, out Type driverType);
    }
}

