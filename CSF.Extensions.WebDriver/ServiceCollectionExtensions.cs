using System;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Factories;
using CSF.Extensions.WebDriver.Identification;
using CSF.Extensions.WebDriver.Proxies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CSF.Extensions.WebDriver
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add web driver factory options, created from configuration.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        internal const string DefaultConfigPath = "WebDriverFactory";

        /// <summary>
        /// Adds/registers the web driver factory and related services for applications which make use of the Options &amp; Configuration
        /// patterns.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When calling this method, there is no need to use <see cref="AddWebDriverFactoryWithoutOptionsPattern(IServiceCollection)"/>, as
        /// this method implicitly adds those services as well.
        /// </para>
        /// <para>
        /// This method enables the use of <see cref="IGetsWebDriver"/> and its implementation <see cref="WebDriverFactory"/>: A 'universal WebDriver
        /// factory'. This also adds an <see cref="IOptions{TOptions}"/> for <see cref="WebDriverCreationOptionsCollection"/> to dependency injection,
        /// using the <see cref="IConfiguration"/> present in the application.
        /// </para>
        /// <para>
        /// If the <paramref name="configPath"/> is not specified, then the options will be created from a configuration path named <c>WebDriverFactory</c>,
        /// relative to the root of the application's configuration.
        /// </para>
        /// <para>
        /// It is also perfectly normal to omit the <paramref name="configureOptions"/> parameter, if the configuration will provide all of the information
        /// required to create the options collection object.
        /// </para>
        /// <para>
        /// An important technique to remember is that the <see cref="IConfiguration"/> may be populated from more than one source.
        /// For example, in a Microsoft Generic Host application, it is default behaviour to receive configuration from both an <c>appsettings.json</c>
        /// as well as supplemental command-line parameters and/or environment variables.
        /// That technique is very useful for setting the value of <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>.
        /// The options collection in a configuration file may contain all of the WebDriver configurations that your application would use in any scenario, and may
        /// use a single command-line parameter or environment variable to choose which of those WebDriver configurations should be activated.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection to which the options should be added.</param>
        /// <param name="configPath">An optional path from the root of the configuration; if <see langword="null" /> then the default value of <c>WebDriverFactory</c> will be used.</param>
        /// <param name="configureOptions">An optional method body which may inspect or alter the options after it has been bound from the configuration.</param>
        /// <returns>The service collection, so that calls may be chained.</returns>
        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services,
                                                             string configPath = DefaultConfigPath,
                                                             Action<WebDriverCreationOptionsCollection> configureOptions = null)
        {
            AddWebDriverFactoryWithoutOptionsPattern(services);

            services.AddTransient<IGetsWebDriver, WebDriverFactory>();
            services.AddTransient(GetOptionsConfigService(configPath, configureOptions));
            services.AddOptions<WebDriverCreationOptionsCollection>().Configure(configureOptions ?? (o => {}));

            return services;
        }

        /// <summary>
        /// Adds/registers the web driver factory and related services for applications which make use of the Options &amp; Configuration
        /// patterns, using a specified configuration section.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When calling this method, there is no need to use <see cref="AddWebDriverFactoryWithoutOptionsPattern(IServiceCollection)"/>, as
        /// this method implicitly adds those services as well.
        /// </para>
        /// <para>
        /// This method enables the use of <see cref="IGetsWebDriver"/> and its implementation <see cref="WebDriverFactory"/>: A 'universal WebDriver
        /// factory'. This also adds an <see cref="IOptions{TOptions}"/> for <see cref="WebDriverCreationOptionsCollection"/> to dependency injection.
        /// </para>
        /// <para>
        /// This overload expects the specified configuration section to correspond directly with the root of the
        /// <see cref="WebDriverCreationOptionsCollection"/> to be bound.
        /// </para>
        /// <para>
        /// It is perfectly normal to omit the <paramref name="configureOptions"/> parameter, if the configuration will provide all of the information
        /// required to create the options collection object.
        /// </para>
        /// <para>
        /// An important technique to remember is that the <see cref="IConfiguration"/> may be populated from more than one source.
        /// For example, in a Microsoft Generic Host application, it is default behaviour to receive configuration from both an <c>appsettings.json</c>
        /// as well as supplemental command-line parameters and/or environment variables.
        /// That technique is very useful for setting the value of <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>.
        /// The options collection in a configuration file may contain all of the WebDriver configurations that your application would use in any scenario, and may
        /// use a single command-line parameter or environment variable to choose which of those WebDriver configurations should be activated.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection to which the services should be added.</param>
        /// <param name="configSection">A configuration section from which the <see cref="WebDriverCreationOptionsCollection"/> should be bound.</param>
        /// <param name="configureOptions">An optional method body which may inspect or alter the options after it has been bound from the configuration.</param>
        /// <returns>The service collection, so that calls may be chained.</returns>
        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services,
                                                             IConfigurationSection configSection,
                                                             Action<WebDriverCreationOptionsCollection> configureOptions = null)
        {
            AddWebDriverFactoryWithoutOptionsPattern(services);

            services.AddTransient<IGetsWebDriver, WebDriverFactory>();
            services.AddTransient(GetOptionsConfigService(configSection, configureOptions));
            services.AddOptions<WebDriverCreationOptionsCollection>().Configure(configureOptions ?? (o => {}));

            return services;
        }

        /// <summary>
        /// Adds/registers the service types required to make use of a WebDriver factory which does not depend upon the
        /// Options &amp; Configuration patterns.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use this method if your application does not make use of either <c>Microsoft.Extensions.Options</c> or
        /// <c>Microsoft.Extensions.Configuration</c>. You will not be able to use <see cref="IGetsWebDriver"/> or
        /// <see cref="WebDriverFactory"/> but you will be able to use <see cref="ICreatesWebDriverFromOptions"/> and
        /// its implementation <see cref="WebDriverFromOptionsFactory"/>.
        /// </para>
        /// <para>
        /// If you do use the Options &amp; Configuration patterns then use an overload of
        /// <see cref="AddWebDriverFactory(IServiceCollection, string, Action{WebDriverCreationOptionsCollection})"/>
        /// instead and do not use this method.  The <c>UseWebDriverFactory</c> methods implicitly include all of the services
        /// which are included here.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection to which the services should be added.</param>
        /// <returns>The service collection, so that calls may be chained.</returns>
        public static IServiceCollection AddWebDriverFactoryWithoutOptionsPattern(this IServiceCollection services)
        {
            services.AddSingleton<IGetsWebDriverAndOptionsTypes, WebDriverTypesProvider>();
            services.AddSingleton<IProxyGenerator, ProxyGenerator>();

            services.AddTransient<IGetsWebDriverWithDeterministicOptionsTypes, SeleniumDriverAndOptionsScanner>();
            services.AddTransient<ICreatesWebDriverFromOptions, WebDriverFromOptionsFactory>();
            services.AddTransient<RemoteWebDriverFromOptionsFactory>();
            services.AddTransient<IGetsBrowserIdFromWebDriver, BrowserIdFactory>();
            services.AddTransient<IGetsBrowserInfoMatch, BrowserInfoMatcher>();
            services.AddTransient<IGetsProxyWebDriver, WebDriverProxyFactory>();
            services.AddTransient<IdentificationAugmenter>();
            services.AddTransient<UnproxyingAugmenter>();

            return services;
        }

        static Func<IServiceProvider, IConfigureOptions<WebDriverCreationOptionsCollection>> GetOptionsConfigService(string configPath,
                                                                                                                     Action<WebDriverCreationOptionsCollection> configureOptions)
        {
            return services =>
            {
                IConfiguration configSection = services.GetRequiredService<IConfiguration>().GetSection(configPath);
                return ActivatorUtilities.CreateInstance<WebDriverCreationConfigureOptions>(services, configSection);
            };
        }

        static Func<IServiceProvider, IConfigureOptions<WebDriverCreationOptionsCollection>> GetOptionsConfigService(IConfiguration configSection,
                                                                                                                     Action<WebDriverCreationOptionsCollection> configureOptions)
        {
            return services =>
            {
                return ActivatorUtilities.CreateInstance<WebDriverCreationConfigureOptions>(services, configSection, configureOptions);
            };
        }
    }
}

