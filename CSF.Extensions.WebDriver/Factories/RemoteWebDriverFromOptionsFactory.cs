using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Factory service which creates implementations of <see cref="RemoteWebDriver"/> from <see cref="WebDriverCreationOptions"/>.
    /// </summary>
    public class RemoteWebDriverFromOptionsFactory : ICreatesWebDriverFromOptions
    {
        readonly ILogger<RemoteWebDriverFromOptionsFactory> logger;

        /// <inheritdoc/>
        public IWebDriver GetWebDriver(WebDriverCreationOptions options, Action<DriverOptions> supplementaryConfiguration = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            var driverOptions = options.OptionsFactory();
            supplementaryConfiguration?.Invoke(driverOptions);

            if(string.IsNullOrWhiteSpace(options.GridUrl))
            {
                var driver = new RemoteWebDriver(driverOptions);
                logger.LogInformation("Driver created for {RemoteDriver}: {Driver}", nameof(RemoteWebDriver), driver);
                return driver;
            }
            else
            {
                var driver = new RemoteWebDriver(new Uri(options.GridUrl), driverOptions);
                logger.LogInformation("Driver created for {RemoteDriver}: {Driver}, Remote URL: {GridUrl}", nameof(RemoteWebDriver), driver, options.GridUrl);
                return driver;
            }
        }

        /// <summary>
        /// Initialises a new instance of <see cref="RemoteWebDriverFromOptionsFactory"/>.
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/> is <see langword="null" />.</exception>
        public RemoteWebDriverFromOptionsFactory(ILogger<RemoteWebDriverFromOptionsFactory> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}