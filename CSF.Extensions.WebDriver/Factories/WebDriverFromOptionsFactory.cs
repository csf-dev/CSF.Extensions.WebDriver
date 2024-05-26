using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Factory service which creates implementations of <see cref="IWebDriver"/> from <see cref="WebDriverCreationOptions"/>.
    /// </summary>
    public class WebDriverFromOptionsFactory : ICreatesWebDriverFromOptions
    {
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly IServiceProvider services;

        /// <inheritdoc/>
        public IWebDriver GetWebDriver(WebDriverCreationOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));

            if (!string.IsNullOrEmpty(options.DriverFactoryType))
                return GetWebDriverFromThirdPartyFactory(options);

            var driverType = typeProvider.GetWebDriverType(options.DriverType);
            if (driverType == typeof(RemoteWebDriver))
                return GetRemoteWebDriver(options);

            if (!driverType.GetConstructors().Any(SeleniumDriverAndOptionsScanner.OptionsConstructorPredicate))
                throw new ArgumentException($"The WebDriver type {driverType.FullName} does not offer a public constructor which takes a single parameter " +
                                            $"of type {nameof(DriverOptions)} (or a derived type). This WebDriver type will require a custom factory: " +
                                            $"{nameof(WebDriverCreationOptions)}.{nameof(WebDriverCreationOptions.DriverFactoryType)}.",
                                            nameof(options));

            return (IWebDriver) Activator.CreateInstance(driverType, options.Options);
        }

        IWebDriver GetRemoteWebDriver(WebDriverCreationOptions options)
            => services.GetRequiredService<RemoteWebDriverFromOptionsFactory>().GetWebDriver(options);

        IWebDriver GetWebDriverFromThirdPartyFactory(WebDriverCreationOptions options)
        {
            var factoryType = typeProvider.GetWebDriverFactoryType(options.DriverFactoryType);
            var factory = GetThirdPartyFactory(factoryType);
            return factory.GetWebDriver(options);
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
        /// Initialises a new instance of <see cref="WebDriverFromOptionsFactory"/>.
        /// </summary>
        /// <param name="typeProvider">A type provider</param>
        /// <param name="services">DI services</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        public WebDriverFromOptionsFactory(IGetsWebDriverAndOptionsTypes typeProvider, IServiceProvider services)
        {
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}