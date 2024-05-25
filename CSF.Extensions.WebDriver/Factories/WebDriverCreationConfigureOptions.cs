using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// A service which configures an <see cref="IOptions{TOptions}"/> of <see cref="WebDriverCreationOptionsCollection"/> within
    /// dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By design, the <see cref="Configure(WebDriverCreationOptionsCollection)"/> method of this class will avoid throwing exceptions.
    /// Instead, if the configuration is unsuitable or not valid then this class will log an error and omit the troublesome driver configuration.
    /// </para>
    /// </remarks>
    public sealed class WebDriverCreationConfigureOptions : IConfigureOptions<WebDriverCreationOptionsCollection>
    {
        readonly IConfiguration configuration;
        readonly Action<WebDriverCreationOptionsCollection> configureOptions;
        readonly ILogger<WebDriverCreationConfigureOptions> logger;
        readonly IGetsWebDriverWithDeterministicOptionsTypes deterministicWebDriverTypesScanner;
        readonly object
            webDriverAndDeterministicOptionsTypesSyncRoot = new object(),
            supportedShorthandDriverTypesSyncRoot = new object();

        /// <summary>
        /// A cache of the implementations of <see cref="IWebDriver"/> which are shipped with Selenium, and the
        /// implementation types of <see cref="DriverOptions"/> which they use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The keys to this dictionary are the web driver types.
        /// This field should only ever be set by <see cref="GetWebDriverAndDeterministicOptionsTypes"/>, which ensures thread-safety.
        /// </para>
        /// </remarks>
        IReadOnlyDictionary<Type, WebDriverAndOptionsTypePair> webDriverAndDeterministicOptionsTypes;

        /// <summary>
        /// A cache of the implementation types of <see cref="IWebDriver"/> for which this library supports the use of shorthand names.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Shorthand names refers to simply the unqualified type name, like <c>ChromeDriver</c>, instead of an assembly-qualified type name.
        /// This field should only ever be set by <see cref="GetSupportedShorthandWebDriverTypes"/>, which ensures thread-safety.
        /// </para>
        /// </remarks>
        IReadOnlyCollection<Type> supportedShorthandDriverTypes;

        /// <inheritdoc/>
        public void Configure(WebDriverCreationOptionsCollection options)
        {
            if(configuration is null)
            {
                logger.LogWarning("Configuration for {TypeName} is null; the WebDriver creation options will be left unconfigured. " +
                                  "Reminder: By default the configuration path is '{Path}'.",
                                  nameof(WebDriverCreationOptionsCollection),
                                  ServiceCollectionExtensions.DefaultConfigPath);
                return;
            }

            options.SelectedConfiguration = configuration.GetValue<string>(nameof(WebDriverCreationOptionsCollection.SelectedConfiguration));

            var driverConfigsSection = configuration.GetSection(nameof(WebDriverCreationOptionsCollection.DriverConfigurations));
            if(driverConfigsSection != null) options.DriverConfigurations = GetDriverConfigurations(driverConfigsSection);

            configureOptions?.Invoke(options);
        }

        static Action<object> GetConfigChangeCallback(WebDriverCreationOptionsCollection options)
        {
            return conf =>
            {
                var config = (IConfiguration) conf;
                options.SelectedConfiguration = config.GetValue<string>(nameof(WebDriverCreationOptionsCollection.SelectedConfiguration));
            };
        }

        IDictionary<string, WebDriverCreationOptions> GetDriverConfigurations(IConfigurationSection configuration)
            => configuration.GetChildren().Select(c => new { c.Key, Value = GetDriverConfiguration(c) }).Where(x => x.Value != null).ToDictionary(k => k.Key, v => v.Value);

        WebDriverCreationOptions GetDriverConfiguration(IConfigurationSection configuration)
        {
            var creationOptions = new WebDriverCreationOptions
            {
                DriverType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.DriverType)),
                OptionsType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.OptionsType)),
                GridUrl = configuration.GetValue<string>(nameof(WebDriverCreationOptions.GridUrl)),
                DriverFactoryType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.DriverFactoryType)),
            };

            if(creationOptions.DriverType is null)
            {
                logger.LogError("{ParamName} is mandatory for all driver configurations; the configuration '{ConfigKey}' will be omitted.",
                                nameof(WebDriverCreationOptions.DriverType),
                                configuration.Key);
                return null;
            }

            var driverType = GetDriverType(creationOptions.DriverType);
            if (driverType is null)
            {
                logger.LogError("No implementation of {WebDriverInterface} was found for the type name '{DriverType}'. The driver configuration '{ConfigKey}' will be omitted. " +
                                "Reminder: If the driver type is not one which is shipped with Selenium then you must specify an assembly-qualified type name.",
                                nameof(IWebDriver),
                                creationOptions.DriverType,
                                configuration.Key);
                return null;
            }

            try
            {
                var optionsType = GetOptionsType(creationOptions.OptionsType, driverType);
                if(optionsType is null)
                {
                    logger.LogError("{OptionsTypeProp} may only be omitted when the {DriverTypeProp} is a local {WebDriverIface} implementation which implies a deterministic " +
                                    "options type in its constructor. For {DriverType} this is not the case. The driver configuration '{ConfigKey}' will be omitted.",
                                    nameof(WebDriverCreationOptions.OptionsType),
                                    nameof(WebDriverCreationOptions.DriverType),
                                    nameof(IWebDriver),
                                    driverType.FullName,
                                    configuration.Key);
                    return null;
                }

                try
                {
                    creationOptions.Options = GetOptions(optionsType, configuration);
                }
                catch(Exception e)
                {
                    logger.LogError(e,
                                    "An unexpected error occurred creating or binding to the {OptionsClass} type {OptionsType}. The driver configuration '{ConfigKey}' will be omitted.",
                                    nameof(DriverOptions),
                                    optionsType.FullName,
                                    configuration.Key);
                    return null;
                }
                
            }
            catch(TypeLoadException e)
            {
                logger.LogError(e,
                                "An unexpected error occurred loading the options type; the driver configuration '{ConfigKey}' will be omitted.",
                                configuration.Key);
                return null;
            }

            return creationOptions;
        }

        /// <summary>
        /// Uses an instance of <see cref="SeleniumDriverAndOptionsScanner"/> to get the implementations of
        /// <see cref="IWebDriver"/> which have deterministic options types.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method makes use of caching in order to avoid repeating the assembly-scanning technique.  That should be
        /// safe to do, because unless we dynamically unload the Selenium assembly and then reload a different one, the
        /// results of this assembly-scanning cannot change during the application lifetime.
        /// </para>
        /// <para>
        /// Thread-safety of the cache is made certain by locking upon <see cref="webDriverAndDeterministicOptionsTypesSyncRoot"/>.
        /// </para>
        /// </remarks>
        /// <returns>A collection which lists all of the implementations of <see cref="IWebDriver"/> which have deterministic options types.</returns>
        /// <seealso cref="SeleniumDriverAndOptionsScanner"/>
        IReadOnlyDictionary<Type, WebDriverAndOptionsTypePair> GetWebDriverAndDeterministicOptionsTypes()
        {
            lock(webDriverAndDeterministicOptionsTypesSyncRoot)
            {
                return webDriverAndDeterministicOptionsTypes = webDriverAndDeterministicOptionsTypes ?? deterministicWebDriverTypesScanner
                    .GetWebDriverAndDeterministicOptionsTypes()
                    .ToDictionary(k => k.WebDriverType, v => v);
            }
        }

        /// <summary>
        /// Gets a collection of the implementation types of <see cref="IWebDriver"/> for which we support shorthand names.
        /// </summary>
        /// <returns>A collection of types.</returns>
        IReadOnlyCollection<Type> GetSupportedShorthandWebDriverTypes()
        {
            lock(supportedShorthandDriverTypesSyncRoot)
            {
                return supportedShorthandDriverTypes = supportedShorthandDriverTypes ?? GetWebDriverAndDeterministicOptionsTypes()
                    .Keys
                    .Union(new[] { typeof(RemoteWebDriver) })
                    .ToArray();
            }
        }

        Type GetDriverType(string driverTypeName)
        {
            var driverType = GetSupportedShorthandWebDriverTypes().FirstOrDefault(x => x.Name == driverTypeName) ?? Type.GetType(driverTypeName);
            return typeof(IWebDriver).IsAssignableFrom(driverType) ? driverType : null;
        }

        Type GetOptionsType(string optionsTypeName, Type driverType)
        {
            var deterministicOptionsTypes = GetWebDriverAndDeterministicOptionsTypes();

            if (optionsTypeName != null)
            {
                var matchedShorthandOptionsType = deterministicOptionsTypes.Values
                    .Select(x => x.OptionsType)
                    .FirstOrDefault(x => Equals(x.Name, optionsTypeName));
                if (matchedShorthandOptionsType != null) return matchedShorthandOptionsType;

                try
                {
                    return Type.GetType(optionsTypeName, true);
                }
                catch(Exception e)
                {
                    throw new TypeLoadException($"The type specified in {nameof(WebDriverCreationOptions.OptionsType)}: '{optionsTypeName}' could not be loaded.", e);
                }
            }

            return deterministicOptionsTypes.TryGetValue(driverType, out var typePair) ? typePair.OptionsType : null;
        }

        static DriverOptions GetOptions(Type optionsType, IConfigurationSection config)
        {
            var options = (DriverOptions) Activator.CreateInstance(optionsType);
            config.Bind(nameof(WebDriverCreationOptions.Options), options);
            return options;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverCreationConfigureOptions"/>.
        /// </summary>
        /// <param name="deterministicWebDriverTypesScanner">The deterministic types scanner.</param>
        /// <param name="configuration">The app configuration.</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverCreationConfigureOptions(IGetsWebDriverWithDeterministicOptionsTypes deterministicWebDriverTypesScanner,
                                                 IConfiguration configuration,
                                                 Action<WebDriverCreationOptionsCollection> configureOptions,
                                                 ILogger<WebDriverCreationConfigureOptions> logger)
        {
            this.deterministicWebDriverTypesScanner = deterministicWebDriverTypesScanner ?? throw new ArgumentNullException(nameof(deterministicWebDriverTypesScanner));
            this.configuration = configuration;
            this.configureOptions = configureOptions;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}