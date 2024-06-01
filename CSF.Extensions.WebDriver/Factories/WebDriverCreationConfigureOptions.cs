using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;

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
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly IConfiguration configuration;
        readonly ILogger<WebDriverCreationConfigureOptions> logger;

        /// <inheritdoc/>
        public void Configure(WebDriverCreationOptionsCollection options)
        {
            if(configuration is null)
            {
                logger.LogWarning("Configuration for {TypeName} is null; the WebDriver creation options will be left unconfigured. " +
                                  "Reminder: By default the configuration path is '{Path}'.",
                                  nameof(WebDriverCreationOptionsCollection),
                                  ServiceCollectionExtensions.FactoryConfigPath);
                return;
            }

            options.SelectedConfiguration = configuration.GetValue<string>(nameof(WebDriverCreationOptionsCollection.SelectedConfiguration));

            var driverConfigsSection = configuration.GetSection(nameof(WebDriverCreationOptionsCollection.DriverConfigurations));
            if(driverConfigsSection != null) options.DriverConfigurations = GetDriverConfigurations(driverConfigsSection);
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

            Type driverType;
            try
            {
                driverType = typeProvider.GetWebDriverType(creationOptions.DriverType);
            }
            catch(Exception e)
            {
                logger.LogError(e,
                                "No implementation of {WebDriverIface} could be found for the {DriverTypeProp} '{DriverType}'; the driver configuration '{ConfigKey}' will be omitted. " +
                                "Reminder: If the driver type is not one which is shipped with Selenium then you must specify its assembly-qualified type name.",
                                nameof(IWebDriver),
                                nameof(WebDriverCreationOptions.DriverType),
                                creationOptions.DriverType,
                                configuration.Key);
                return null;
            }

            Type optionsType;
            try
            {
                optionsType = typeProvider.GetWebDriverOptionsType(driverType, creationOptions.OptionsType);                
            }
            catch(Exception e)
            {
                logger.LogError(e,
                                "No type deriving from {OptionsBase} could be found for the combination of {WebDriverIface} {DriverType} and {OptionsTypeProp} '{OptionsType}'; the configuration '{ConfigKey}' will be omitted. " +
                                "See the exception details for more information.",
                                nameof(DriverOptions),
                                nameof(IWebDriver),
                                driverType.Name,
                                nameof(WebDriverCreationOptions.OptionsType),
                                creationOptions.OptionsType,
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
                                "An unexpected error occurred creating or binding to the {OptionsClass} type {OptionsType}; the configuration '{ConfigKey}' will be omitted.",
                                nameof(DriverOptions),
                                optionsType.FullName,
                                configuration.Key);
                return null;
            }

            return creationOptions;
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
        /// <param name="typeProvider">A type-loading utility class.</param>
        /// <param name="configuration">The app configuration.</param>
        /// <param name="logger">A logging implementation.</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverCreationConfigureOptions(IGetsWebDriverAndOptionsTypes typeProvider,
                                                 IConfiguration configuration,
                                                 ILogger<WebDriverCreationConfigureOptions> logger)
        {
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.configuration = configuration;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}