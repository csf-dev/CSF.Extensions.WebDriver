using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Chain of responsibility implementation of <see cref="ICreatesWebDriverFromOptions"/> which creates instances of
    /// <see cref="RemoteWebDriver"/>.
    /// </summary>
    public class RemoteWebDriverFromOptionsFactory : ICreatesWebDriverFromOptions
    {
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly ILogger logger;
        readonly ICreatesWebDriverFromOptions next;

        /// <inheritdoc/>
        public WebDriverAndOptions GetWebDriver(WebDriverCreationOptions options, Action<DriverOptions> supplementaryConfiguration = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            var driverType = typeProvider.GetWebDriverType(options.DriverType);
            if (driverType != typeof(RemoteWebDriver))
                return next.GetWebDriver(options, supplementaryConfiguration);

            logger.LogDebug("Creating a {RemoteDriver}", nameof(RemoteWebDriver));

            var driverOptions = options.OptionsFactory();
            supplementaryConfiguration?.Invoke(driverOptions);

            if(string.IsNullOrWhiteSpace(options.GridUrl))
            {
                var driver = new RemoteWebDriver(driverOptions);
                logger.LogInformation("Driver created for {RemoteDriver}: {Driver}", nameof(RemoteWebDriver), driver);
                return new WebDriverAndOptions(driver, driverOptions);
            }
            else
            {
                var driver = new RemoteWebDriver(new Uri(options.GridUrl), driverOptions);
                logger.LogInformation("Driver created for {RemoteDriver}: {Driver}, Remote URL: {GridUrl}", nameof(RemoteWebDriver), driver, options.GridUrl);
                return new WebDriverAndOptions(driver, driverOptions);
            }
        }

        /// <summary>
        /// Initialises a new instance of <see cref="RemoteWebDriverFromOptionsFactory"/>.
        /// </summary>
        /// <param name="typeProvider">A type-providing service</param>
        /// <param name="logger">A logger</param>
        /// <param name="next">The next service in the chain of responsibility.</param>
        /// <exception cref="ArgumentNullException">If any parameters is <see langword="null" />.</exception>
        public RemoteWebDriverFromOptionsFactory(IGetsWebDriverAndOptionsTypes typeProvider,
                                                 ILogger<RemoteWebDriverFromOptionsFactory> logger,
                                                 ICreatesWebDriverFromOptions next)
        {
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }
    }
}