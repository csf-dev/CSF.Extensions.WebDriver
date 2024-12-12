using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Factory service which creates implementations of <see cref="IWebDriver"/> from <see cref="WebDriverCreationOptions"/>.
    /// </summary>
    public class WebDriverFromOptionsFactory : ICreatesWebDriverFromOptions
    {
        static readonly MethodInfo customizeGenericMethod = typeof(WebDriverFromOptionsFactory).GetMethod(nameof(CuztomizeOptionsGeneric), BindingFlags.NonPublic | BindingFlags.Static);

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
            CustomizeOptions(driverOptions, options.OptionsCustomizer);
            supplementaryConfiguration?.Invoke(driverOptions);
            var driver = (IWebDriver) Activator.CreateInstance(driverType, driverOptions);
            logger.LogInformation("Driver created via reflection: {Driver}", driver);
            return new WebDriverAndOptions(driver, driverOptions);
        }

        static void CustomizeOptions(DriverOptions options, object customizer)
        {
            if (customizer is null) return;
            var method = customizeGenericMethod.MakeGenericMethod(options.GetType());
            method.Invoke(null, [options, customizer]);
        }

        static void CuztomizeOptionsGeneric<TOptions>(TOptions options, object customizer)
            where TOptions : DriverOptions
        {
            if (!(customizer is ICustomizesOptions<TOptions> customizerInstance))
                throw new ArgumentException($"The customizer type {customizer.GetType().FullName} does not implement {customizeGenericMethod.Name}<{typeof(TOptions).Name}>.",
                                            nameof(customizer));

            customizerInstance.CustomizeOptions(options);
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