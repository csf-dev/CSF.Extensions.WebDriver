using OpenQA.Selenium;
using Microsoft.Extensions.Options;
using CSF.Extensions.WebDriver.Factories;

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
        /// <returns>A WebDriver</returns>
        IWebDriver GetDefaultWebDriver();

        /// <summary>
        /// Gets an <see cref="IWebDriver"/> using the named driver configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="configurationName"/> must correspond to a key within the <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/>
        /// collection.
        /// </para>
        /// </remarks>
        /// <param name="configurationName">The driver configuration name.</param>
        /// <returns>A WebDriver</returns>
        IWebDriver GetWebDriver(string configurationName);
    }
}