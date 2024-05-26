using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// A service that gets an instance of <see cref="IWebDriver"/> from a configuration object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service could be thought of as 'a general WebDriver factory'.
    /// This allows the creation of any implementation of WebDriver from just configuration information.
    /// </para>
    /// <para>
    /// The purpose of this service is to make it easier to write applications (or browser-based tests) which
    /// test using a variety of WebDriver implementations (such as a variety of browsers).
    /// By using this service, the application or test need not hard-code the logic to create the WebDriver
    /// instances, they can be input from configuration data.
    /// </para>
    /// </remarks>
    public interface ICreatesWebDriverFromOptions
    {
        /// <summary>
        /// Gets a WebDriver using the settings from the specified WebDriver configuration object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WebDriver configuration object specifies both the <see cref="IWebDriver"/> implementation
        /// type to use as well as any relevant options for that WebDriver.
        /// </para>
        /// </remarks>
        /// <param name="options">An object indicating which WebDriver implementation to use and how the WebDriver should be configured.</param>
        /// <returns>A WebDriver instance</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="options"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// If any of:
        /// <list type="bullet">
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverType"/> of the <paramref name="options"/> is <see langword="null" /> or empty</description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.Options"/> of the <paramref name="options"/> is <see langword="null" /></description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverType"/> of the <paramref name="options"/> is set to an implementation of
        /// <see cref="IWebDriver"/> which does not expose a public constructor which takes a single parameter of type <see cref="DriverOptions"/> (or a more
        /// derived type).  Such WebDriver implementations will require a custom factory, indicated by <see cref="WebDriverCreationOptions.DriverFactoryType"/></description></item>.
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverFactoryType"/> of the <paramref name="options"/> is not null, but the implementation of
        /// <see cref="ICreatesWebDriverFromOptions"/> indicated by that type did not return an <see cref="IWebDriver"/> instance</description></item>
        /// </list>
        /// </exception>
        /// <exception cref="TypeLoadException">
        /// Either <see cref="WebDriverCreationOptions.DriverType"/> or <see cref="WebDriverCreationOptions.DriverFactoryType"/> of the
        /// <paramref name="options"/> are non-null/non-empty but no type can be found matching the specifed values.
        /// </exception>
        IWebDriver GetWebDriver(WebDriverCreationOptions options);
    }
}