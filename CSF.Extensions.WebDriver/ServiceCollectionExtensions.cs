using System;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Factories;
using CSF.Extensions.WebDriver.Identification;
using CSF.Extensions.WebDriver.Proxies;
using CSF.Extensions.WebDriver.Quirks;
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
        internal const string
            FactoryConfigPath = "WebDriverFactory",
            QuirksConfigPath  = "WebDriverQuirks";

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
                                                             string configPath = FactoryConfigPath,
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
            services.AddTransient(s =>
            {
                // Chain of responsibility/decorator stack
                ICreatesWebDriverFromOptions output = s.GetRequiredService<WebDriverFromOptionsFactory>();
                output = ActivatorUtilities.CreateInstance<RemoteWebDriverFromOptionsFactory>(s, output);
                output = ActivatorUtilities.CreateInstance<WebDriverFromThirdPartyFactory>(s, output);
                output = ActivatorUtilities.CreateInstance<ProxyWrappingWebDriverFactoryDecorator>(s, output);
                return output;
            });
            services.AddTransient<WebDriverFromOptionsFactory>();
            services.AddTransient<RemoteWebDriverFromOptionsFactory>();
            services.AddTransient<WebDriverFromThirdPartyFactory>();
            services.AddTransient<ProxyWrappingWebDriverFactoryDecorator>();
            services.AddTransient<IGetsBrowserIdFromWebDriver, BrowserIdFactory>();
            services.AddTransient<IGetsBrowserInfoMatch, BrowserInfoMatcher>();
            services.AddTransient<IGetsProxyWebDriver, WebDriverProxyFactory>();
            services.AddTransient<IdentificationAugmenter>();
            services.AddTransient<UnproxyingAugmenter>();

            return services;
        }

        /// <summary>
        /// Adds services to depenency injection which support the 'browser quirks' infrastructure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Either <paramref name="quirksData"/> must not be <see langword="null" /> or <paramref name="useOptions"/> must be
        /// <see langword="true" /> or else an exception will be raised from this method.
        /// </para>
        /// <para>
        /// If you wish to make use of the browser-quirks functionality provided by this library then this method is used to
        /// set that functionality up within dependency injection.  This method permits the usage of up to two sources of information
        /// for the <see cref="QuirksData"/> which shall be used to indicate which browsers/versions are affected by which quirks:
        /// </para>
        /// <list type="bullet">
        /// <item><description>A static source of data, provided via <paramref name="quirksData"/></description></item>
        /// <item><description>Data coming from the Microsoft Options Pattern, if <paramref name="useOptions"/> is set to
        /// <see langword="true" /> (this is the default value)</description></item>
        /// </list>
        /// <para>
        /// The purpose of using two sources of data for the quirks is described in more depth in the remarks to
        /// <see cref="QuirksDataProvider"/> but in short it allows library authors to provide some static quirks data which may be
        /// supplemented and/or overridden (in part or completely) by user-specified options.
        /// </para>
        /// <para>
        /// If you wish to use <see cref="ProxyCreationOptions.AddQuirks"/> then this method must be used in order to add the required
        /// services to DI.
        /// </para>
        /// </remarks>
        /// <param name="services">The service collection to which the services should be added.</param>
        /// <param name="quirksData">An optional source of static quirks data.</param>
        /// <param name="useOptions">Whether or not to use quirks information from the Microsoft Options Pattern as a source of data,
        /// to either provide all of the quirks, or to supplement/shadow the data in <paramref name="quirksData"/>.</param>
        /// <seealso cref="QuirksDataProvider"/>
        /// <seealso cref="IHasQuirks"/>
        /// <exception cref="ArgumentException">If both <paramref name="quirksData"/> is <see langword="null" /> and <paramref name="useOptions"/> is <see langword="false" />.</exception>
        public static IServiceCollection AddQuirksServices(this IServiceCollection services, QuirksData quirksData = null, bool useOptions = true)
        {
            if (quirksData is null && !useOptions)
                throw new ArgumentException("Either some non-null quirks data must be specified or the options pattern must be activated. If neither are activated then this would lead to always-empty/useless WebDriver quirks data, which is not a supported use of this functionality.");

            services.AddTransient<IGetsQuirksForBrowserId, ApplicableQuirksProvider>();
            services.AddTransient<QuirksAugmenter>();
            services.AddTransient<QuirksInterceptor>();
            if (useOptions)
                services.AddOptions<QuirksData>().BindConfiguration(QuirksConfigPath);
            
            services.AddSingleton<IGetsQuirksData>(s =>
            {
                if (!useOptions) return new QuirksDataProvider(quirksData);
                return ActivatorUtilities.CreateInstance<QuirksDataProvider>(s, new[] { quirksData ?? QuirksData.Empty });
            });

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

