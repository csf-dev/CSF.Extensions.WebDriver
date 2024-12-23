using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Extension methods for <see cref="ICreatesWebDriverFromOptions"/>.
    /// </summary>
    public static class WebDriverFactoryExtensions
    {
        /// <summary>
        /// Gets a WebDriver using the selected entry of a web drivers configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="WebDriverCreationOptionsCollection"/> instance must have a non-null/non-empty <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>
        /// and there must be an entry in <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> of the same name as that selected configuration.
        /// That is the configuration instance which is treated as the "selected driver configuration".  It will be passed to
        /// <see cref="ICreatesWebDriverFromOptions.GetWebDriver(WebDriverCreationOptions,Action{DriverOptions})"/> and the result returned from this method.
        /// </para>
        /// <para>
        /// The selected driver configuration object specifies both the <see cref="IWebDriver"/> implementation
        /// type to use as well as any relevant options for that WebDriver.
        /// </para>
        /// </remarks>
        /// <param name="factory">The WebDriver factory.</param>
        /// <param name="configuration">An object which contains one or more WebDriver configurations as well as a value indicating which
        /// of those configurations is currently selected.</param>
        /// <param name="supplementaryConfiguration">An optional action which further-configures the WebDriver options before the driver is created.</param>
        /// <returns>An object containing both a WebDriver and the options which were used to create it.</returns>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// If any of:
        /// <list type="bullet">
        /// <item><description>The selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> entry is <see langword="null" /></description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverType"/> of the selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> is <see langword="null" /> or empty</description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.OptionsFactory"/> of the selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> is <see langword="null" /></description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverType"/> of the selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> is set to <c>RemoteWebDriver</c>
        /// or to a type that is not shipped with Selenium but the <see cref="WebDriverCreationOptions.OptionsType"/> is <see langword="null" /> or empty</description></item>
        /// </list>
        /// </exception>
        /// <exception cref="TypeLoadException">
        /// Either <see cref="WebDriverCreationOptions.DriverType"/> or <see cref="WebDriverCreationOptions.OptionsType"/> of the
        /// selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> are non-null/non-empty but no type can be found matching the specifed values.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/> is <see langword="null" /> or empty and there is not precisely one
        /// configuration within <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/>.
        /// Or if the <paramref name="configuration"/> does not contain a configuration item with a key matching the
        /// <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>.
        /// </exception>
        public static WebDriverAndOptions GetWebDriver(this ICreatesWebDriverFromOptions factory,
                                              WebDriverCreationOptionsCollection configuration,
                                              Action<DriverOptions> supplementaryConfiguration = null)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            var configItem = configuration.GetSelectedConfiguration() ?? throw new ArgumentException($"The selected web driver configuration item must not be null.", nameof(configuration));
            return factory.GetWebDriver(configItem, supplementaryConfiguration);
        }

        /// <summary>
        /// Gets a WebDriver using specified named WebDriver configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There must be an entry in <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> of the same name as <paramref name="driverConfigName"/>.
        /// It will be passed to <see cref="ICreatesWebDriverFromOptions.GetWebDriver(WebDriverCreationOptions,Action{DriverOptions})"/> and the result returned from this method.
        /// </para>
        /// <para>
        /// The selected driver configuration object specifies both the <see cref="IWebDriver"/> implementation
        /// type to use as well as any relevant options for that WebDriver.
        /// </para>
        /// </remarks>
        /// <param name="factory">The WebDriver factory.</param>
        /// <param name="configuration">An object which contains one or more WebDriver configurations.</param>
        /// <param name="driverConfigName">A string indicating the key of <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> will be used to
        /// get a <see cref="WebDriverCreationOptions"/> for creating the driver.</param>
        /// <param name="supplementaryConfiguration">An optional action which further-configures the WebDriver options before the driver is created.</param>
        /// <returns>An object containing both a WebDriver and the options which were used to create it.</returns>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">
        /// If any of:
        /// <list type="bullet">
        /// <item><description><paramref name="driverConfigName"/> does not match any key of <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/>
        /// for <paramref name="configuration"/>.</description></item>
        /// <item><description>The selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> entry is <see langword="null" /></description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverType"/> of the selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> is <see langword="null" /> or empty</description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.OptionsFactory"/> of the selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> is <see langword="null" /></description></item>
        /// <item><description>The <see cref="WebDriverCreationOptions.DriverType"/> of the selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> is set to <c>RemoteWebDriver</c>
        /// or to a type that is not shipped with Selenium but the <see cref="WebDriverCreationOptions.OptionsType"/> is <see langword="null" /> or empty</description></item>
        /// </list>
        /// </exception>
        /// <exception cref="TypeLoadException">
        /// Either <see cref="WebDriverCreationOptions.DriverType"/> or <see cref="WebDriverCreationOptions.OptionsType"/> of the
        /// selected <see cref="WebDriverCreationOptionsCollection.DriverConfigurations"/> are non-null/non-empty but no type can be found matching the specifed values.
        /// </exception>
        public static WebDriverAndOptions GetWebDriver(this ICreatesWebDriverFromOptions factory,
                                              WebDriverCreationOptionsCollection configuration,
                                              string driverConfigName,
                                              Action<DriverOptions> supplementaryConfiguration = null)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            if (string.IsNullOrEmpty(driverConfigName))
                throw new ArgumentException($"The selected configuration name must not be null or empty.", nameof(driverConfigName));
            if (!configuration.DriverConfigurations.TryGetValue(driverConfigName, out WebDriverCreationOptions value) || value is null)
                throw new ArgumentException($"The {nameof(WebDriverCreationOptionsCollection)}.{nameof(WebDriverCreationOptionsCollection.DriverConfigurations)} must contain a non-null entry matching the selected configuration: '{driverConfigName}'. No such entry was found.", nameof(driverConfigName));

            return factory.GetWebDriver(value, supplementaryConfiguration);
        }
    }
}