using System;
using CSF.Extensions.WebDriver.Factories;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver
{
    /// <summary>
    /// A 'universal web driver factory' which uses the options pattern to inject a <see cref="WebDriverCreationOptionsCollection"/>
    /// and forward that information to a. <see cref="ICreatesWebDriverFromOptions"/>.
    /// </summary>
    public class WebDriverFactory : IGetsWebDriver
    {
        readonly IOptions<WebDriverCreationOptionsCollection> options;
        readonly ICreatesWebDriverFromOptions factory;

        /// <inheritdoc/>
        public IWebDriver GetDefaultWebDriver() => factory.GetWebDriver(options.Value);

        /// <inheritdoc/>
        public IWebDriver GetWebDriver(string configurationName) => factory.GetWebDriver(options.Value, configurationName);

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverFactory"/>.
        /// </summary>
        /// <param name="options">The application's options related to creation of web drivers.</param>
        /// <param name="factory">The underlying driver factory</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverFactory(IOptions<WebDriverCreationOptionsCollection> options,
                                ICreatesWebDriverFromOptions factory)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }
}

