using System;
using CSF.Extensions.WebDriver.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add web driver factory options, created from configuration.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        internal const string DefaultConfigPath = "WebDriverFactory";

        /// <summary>
        /// Adds/registers <see cref="IOptions{TOptions}"/> for <see cref="WebDriverCreationOptionsCollection"/>, using the <see cref="IConfiguration"/>
        /// present in the application.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the default mechanism by which the web driver factory options should be added to an application's dependency injection.
        /// If the <paramref name="configPath"/> is not specified, then the options will be created from a configuration path named <c>WebDriverFactory</c>,
        /// relative to the root of the application's configuration.
        /// </para>
        /// <para>
        /// It is also perfectly normal to omit the <paramref name="configureOptions"/> parameter, if the configuration will provide all of the information
        /// required to create the options collection object.
        /// </para>
        /// <para>
        /// An important technique to remember here is that by default, the root <see cref="IConfiguration"/> object will be populated from a number of
        /// sources.  Not only will configuration come from files such as <c>appsettings.json</c>, but it may also be supplemented with command-line
        /// parameters and/or environment variables.
        /// That technique is very useful for setting the value of <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>.
        /// The options collection in a configuration file may contain all of the WebDriver configurations that your application might use, and may
        /// use a single command-line parameter or environment variable (for example) to choose which of those WebDriver configurations should be activated.
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
            AddSupportingServices(services);
            services.AddTransient(GetOptionsConfigService(configPath, configureOptions));

            return services;
        }

        /// <summary>
        /// Adds/registers <see cref="IOptions{TOptions}"/> for <see cref="WebDriverCreationOptionsCollection"/>, using a specified
        /// <see cref="IConfigurationSection"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This overload expects the specified configuration section to correspond directly with the root of the
        /// <see cref="WebDriverCreationOptionsCollection"/> to be bound.
        /// </para>
        /// <para>
        /// It is perfectly normal to omit the <paramref name="configureOptions"/> parameter, if the configuration will provide all of the information
        /// required to create the options collection object.
        /// </para>
        /// <para>
        /// An important technique to remember here is that by default, the root <see cref="IConfiguration"/> object will be populated from a number of
        /// sources.  Not only will configuration come from files such as <c>appsettings.json</c>, but it may also be supplemented with command-line
        /// parameters and/or environment variables.
        /// That technique is very useful for setting the value of <see cref="WebDriverCreationOptionsCollection.SelectedConfiguration"/>.
        /// The options collection in a configuration file may contain all of the WebDriver configurations that your application might use, and may
        /// use a single command-line parameter or environment variable (for example) to choose which of those WebDriver configurations should be activated.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection to which the options should be added.</param>
        /// <param name="configSection">A configuration section from which the <see cref="WebDriverCreationOptionsCollection"/> should be bound.</param>
        /// <param name="configureOptions">An optional method body which may inspect or alter the options after it has been bound from the configuration.</param>
        /// <returns>The service collection, so that calls may be chained.</returns>
        public static IServiceCollection AddWebDriverFactory(this IServiceCollection services,
                                                             IConfigurationSection configSection,
                                                             Action<WebDriverCreationOptionsCollection> configureOptions = null)
        {
            AddSupportingServices(services);
            services.AddTransient(GetOptionsConfigService(configSection, configureOptions));
            
            return services;
        }

        static void AddSupportingServices(IServiceCollection services)
        {
            services.AddSingleton<IGetsWebDriverAndOptionsTypes, WebDriverTypesProvider>();

            services.AddTransient<IGetsWebDriverWithDeterministicOptionsTypes, SeleniumDriverAndOptionsScanner>();
            services.AddTransient<ICreatesWebDriverFromOptions, WebDriverFromOptionsFactory>();
            services.AddTransient<RemoteWebDriverFromOptionsFactory>();

            services.AddOptions<WebDriverCreationOptionsCollection>();
        }

        static Func<IServiceProvider, IConfigureOptions<WebDriverCreationOptionsCollection>> GetOptionsConfigService(string configPath,
                                                                                                                     Action<WebDriverCreationOptionsCollection> configureOptions)
        {
            return services =>
            {
                IConfiguration configSection = services.GetRequiredService<IConfiguration>().GetSection(configPath);
                return ActivatorUtilities.CreateInstance<WebDriverCreationConfigureOptions>(services, configSection, configureOptions);
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

