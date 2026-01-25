using System;
using Microsoft.Extensions.Configuration;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// A service which reads an <see cref="IConfigurationSection"/> which describes a creation-strategy for a WebDriver, and gets an
    /// instance of <see cref="WebDriverCreationOptions"/>.
    /// </summary>
    public interface IParsesSingleWebDriverConfigurationSection
    {
        /// <summary>
        /// Gets an instance of <see cref="WebDriverCreationOptions"/> from the specified <see cref="IConfigurationSection"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that if the configuration is invalid, then this method will return a <see langword="null"/> instance of
        /// <see cref="WebDriverCreationOptions"/>.
        /// </para>
        /// </remarks>
        /// <param name="configuration">The configuration section which describes the configuration of a WebDriver.</param>
        /// <returns>A strongly-typed options object, or a <see langword="null"/> reference indicating an invalid configuration.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="configuration"/> is <see langword="null"/>.</exception>
        WebDriverCreationOptions GetDriverConfiguration(IConfigurationSection configuration);
    }
}

