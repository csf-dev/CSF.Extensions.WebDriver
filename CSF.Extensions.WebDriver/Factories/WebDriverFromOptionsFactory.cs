using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Factory service which creates implementations of <see cref="IWebDriver"/> from <see cref="WebDriverCreationOptions"/>.
    /// </summary>
    public class WebDriverFromOptionsFactory : ICreatesWebDriverFromOptions
    {
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly ILogger logger;

        /// <inheritdoc/>
        public WebDriverAndOptions GetWebDriver(WebDriverCreationOptions options, Action<DriverOptions> supplementaryConfiguration = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            logger.LogDebug("Creating a web driver of type {DriverType} using options type {OptionsType}", options.DriverType, options.OptionsFactory.GetType());

            var driverType = typeProvider.GetWebDriverType(options.DriverType);
            if (!driverType.GetConstructors().Any(SeleniumDriverAndOptionsScanner.OptionsConstructorPredicate))
                throw new ArgumentException($"The WebDriver type {driverType.FullName} does not offer a public constructor which takes a single parameter " +
                                            $"of type {nameof(DriverOptions)} (or a derived type). This WebDriver type will require a custom factory: " +
                                            $"{nameof(WebDriverCreationOptions)}.{nameof(WebDriverCreationOptions.DriverFactoryType)}.",
                                            nameof(options));

            var driverOptions = options.OptionsFactory();
            supplementaryConfiguration?.Invoke(driverOptions);
            var driver = (IWebDriver) Activator.CreateInstance(driverType, driverOptions);
            logger.LogInformation("Driver created via reflection: {Driver}", driver);
            return new WebDriverAndOptions(driver, driverOptions);
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverFromOptionsFactory"/>.
        /// </summary>
        /// <param name="typeProvider">A type provider</param>
        /// <param name="logger">A logger</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        public WebDriverFromOptionsFactory(IGetsWebDriverAndOptionsTypes typeProvider,
                                           ILogger<WebDriverFromOptionsFactory> logger)
        {
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}