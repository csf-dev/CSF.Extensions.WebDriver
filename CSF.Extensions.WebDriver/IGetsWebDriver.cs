using OpenQA.Selenium;
using Microsoft.Extensions.Options;
using CSF.Extensions.WebDriver.Factories;
using System;

namespace CSF.Extensions.WebDriver
{
    /// <summary>
    /// An object which can get an <see cref="IWebDriver"/> from the options available in the application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This integrates with the Microsoft-recommended Options Pattern: https://learn.microsoft.com/en-us/dotnet/core/extensions/options.
    /// The default implementation of this interface will make use of <see cref="IOptions{TOptions}"/> of <see cref="WebDriverCreationOptionsCollection"/>
    /// which provides a 'universal configuration' mechanism for WebDriver creation options.
    /// </para>
    /// </remarks>
    public interface IGetsWebDriver
    {
        /// <summary>
        /// Gets an <see cref="IWebDriver"/> using the driver configuration indicated by the <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="supplementaryConfiguration"/> can be useful for customising the WebDriver options in a manner specific to a single WebDriver.
        /// For example some remote WebDriver providers offer a facility to 'tag' the WebDriver with a current test/scenario name. This is done by adding
        /// additional options to the DriverOptions, specific to that scenario.
        /// </para>
        /// </remarks>
        /// <param name="supplementaryConfiguration">An optional action which further-configures the WebDriver options before the driver is created.</param>
        /// <returns>A WebDriver</returns>
        IWebDriver GetDefaultWebDriver(Action<DriverOptions> supplementaryConfiguration = null);

        /// <summary>
        /// Gets an <see cref="IWebDriver"/> using the named driver configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="configurationName"/> must correspond to a key within the <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/>
        /// collection.
        /// </para>
        /// <para>
        /// The <paramref name="supplementaryConfiguration"/> can be useful for customising the WebDriver options in a manner specific to a single WebDriver.
        /// For example some remote WebDriver providers offer a facility to 'tag' the WebDriver with a current test/scenario name. This is done by adding
        /// additional options to the DriverOptions, specific to that scenario.
        /// </para>
        /// </remarks>
        /// <param name="configurationName">The driver configuration name.</param>
        /// <param name="supplementaryConfiguration">An optional action which further-configures the WebDriver options before the driver is created.</param>
        /// <returns>A WebDriver</returns>
        IWebDriver GetWebDriver(string configurationName, Action<DriverOptions> supplementaryConfiguration = null);
    }
}