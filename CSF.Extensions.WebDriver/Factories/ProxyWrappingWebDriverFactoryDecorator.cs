using System;
using CSF.Extensions.WebDriver.Proxies;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Decorator for implementations of <see cref="ICreatesWebDriverFromOptions"/> which (if applicable) wraps the
    /// returned WebDriver in a proxy object, which adds further functionality.
    /// </summary>
    public class ProxyWrappingWebDriverFactoryDecorator : ICreatesWebDriverFromOptions
    {
        readonly ICreatesWebDriverFromOptions wrapped;
        readonly IGetsProxyWebDriver proxyFactory;
        readonly ILogger logger;

        /// <inheritdoc/>
        public WebDriverAndOptions GetWebDriver(WebDriverCreationOptions options, Action<DriverOptions> supplementaryConfiguration = null)
        {
            var driverAndOptions = wrapped.GetWebDriver(options, supplementaryConfiguration);

            if (!options.AddBrowserIdentification && !options.AddBrowserQuirks)
            {
                logger.LogDebug("Skipping wrapping {Driver} with a proxy, since no functionality is enabled which requires it", driverAndOptions.WebDriver);
                return driverAndOptions;
            }

            var proxyOptions = new ProxyCreationOptions
            {
                AddIdentification = options.AddBrowserIdentification,
                AddQuirks = options.AddBrowserQuirks,
                DriverOptions = driverAndOptions.DriverOptions,
            };

            return new WebDriverAndOptions(proxyFactory.GetProxyWebDriver(driverAndOptions.WebDriver, proxyOptions),
                                           driverAndOptions.DriverOptions);
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ProxyWrappingWebDriverFactoryDecorator"/>.
        /// </summary>
        /// <param name="wrapped">The wrapped factory</param>
        /// <param name="proxyFactory">A proxy factory implementation</param>
        /// <param name="logger">A logger</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        public ProxyWrappingWebDriverFactoryDecorator(ICreatesWebDriverFromOptions wrapped,
                                                      IGetsProxyWebDriver proxyFactory,
                                                      ILogger<ProxyWrappingWebDriverFactoryDecorator> logger)
        {
            this.wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}