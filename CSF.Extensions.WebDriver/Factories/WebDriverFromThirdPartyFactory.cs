using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Chain of responsibility implementation of <see cref="ICreatesWebDriverFromOptions"/> which creates WebDriver instances
    /// using a third-party factory type, specified in <see cref="WebDriverCreationOptions.DriverFactoryType"/>.
    /// </summary>
    public class WebDriverFromThirdPartyFactory : ICreatesWebDriverFromOptions
    {
        readonly ICreatesWebDriverFromOptions next;
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly IServiceProvider services;
        readonly ILogger logger;

        /// <inheritdoc/>
        public WebDriverAndOptions GetWebDriver(WebDriverCreationOptions options, Action<DriverOptions> supplementaryConfiguration = null)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(options.DriverFactoryType))
                return next.GetWebDriver(options, supplementaryConfiguration);
            
            logger.LogDebug("Using factory type {FactoryType} specified in the configuration", options.DriverFactoryType);
            var factoryType = typeProvider.GetWebDriverFactoryType(options.DriverFactoryType);
            var factory = GetThirdPartyFactory(factoryType);
            var driverAndOptions = factory.GetWebDriver(options, supplementaryConfiguration);
            logger.LogInformation("Driver created via third-party factory: {Driver}", driverAndOptions.WebDriver);
            return driverAndOptions;
        }

        ICreatesWebDriverFromOptions GetThirdPartyFactory(Type factoryType)
        {
            try
            {
                return (ICreatesWebDriverFromOptions) (services.GetService(factoryType) ?? Activator.CreateInstance(factoryType));
            }
            catch (Exception e)
            {
                throw new ArgumentException($"The factory type {factoryType.FullName} could not be instantiated, either via DI or " +
                                            $"{nameof(Activator)}.{nameof(Activator.CreateInstance)}. It must either be available through DI " +
                                            "or it must have a public parameterless constructor.",
                                            nameof(factoryType),
                                            e);
            }
        }


        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverFromThirdPartyFactory"/>.
        /// </summary>
        /// <param name="next">The next implementation in the chain of responsibility.</param>
        /// <param name="typeProvider">A type-providing service.</param>
        /// <param name="services">The DI service provider.</param>
        /// <param name="logger">A logger</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        public WebDriverFromThirdPartyFactory(ICreatesWebDriverFromOptions next,
                                              IGetsWebDriverAndOptionsTypes typeProvider,
                                              IServiceProvider services,
                                              ILogger<WebDriverFromThirdPartyFactory> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}