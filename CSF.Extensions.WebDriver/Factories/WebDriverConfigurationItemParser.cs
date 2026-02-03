using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Default implementation of <see cref="IParsesSingleWebDriverConfigurationSection"/>.
    /// </summary>
    public class WebDriverConfigurationItemParser : IParsesSingleWebDriverConfigurationSection
    {
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly ILogger<WebDriverConfigurationItemParser> logger;

        /// <inheritdoc/>
        public WebDriverCreationOptions GetDriverConfiguration(IConfigurationSection configuration)
        {
            if(configuration is null) throw new ArgumentNullException(nameof(configuration));

            var creationOptions = new WebDriverCreationOptions
            {
                DriverType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.DriverType)),
                OptionsType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.OptionsType)),
                GridUrl = configuration.GetValue<string>(nameof(WebDriverCreationOptions.GridUrl)),
                DriverFactoryType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.DriverFactoryType)),
            };

            if(configuration.GetSection(nameof(WebDriverCreationOptions.AddBrowserIdentification)).Exists())
                creationOptions.AddBrowserIdentification = configuration.GetValue<bool>(nameof(WebDriverCreationOptions.AddBrowserIdentification));

            if(configuration.GetSection(nameof(WebDriverCreationOptions.AddBrowserQuirks)).Exists())
                creationOptions.AddBrowserQuirks = configuration.GetValue<bool>(nameof(WebDriverCreationOptions.AddBrowserQuirks));

            if(!TryGetDriverType(creationOptions, configuration, out var driverType))
                return null;

            if(!TryGetOptionsType(creationOptions, configuration, driverType, out var optionsType))
                return null;

            if(!TrySetOptionsCustomizer(creationOptions, configuration, optionsType))
                return null;

            return creationOptions;
        }

        /// <summary>
        /// Validates and gets the <see cref="Type"/> of the implementation of <see cref="IWebDriver"/> implementation indicated by the configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that it is valid for the driver type to be <see langword="null"/> if <see cref="WebDriverCreationOptions.DriverFactoryType"/> is specified.
        /// In that scenario, the driver type is unused, but it still indicates a valid configuration.
        /// </para>
        /// </remarks>
        /// <param name="options">The options, as they have been parsed so far</param>
        /// <param name="configuration">The configuration section</param>
        /// <param name="driverType">If this method returns <see langword="true"/> then this is a <see cref="Type"/> of the web driver, otherwise
        /// this value is undefined and must be ignored.</param>
        /// <returns><see langword="true"/> if the driver type information is valid; <see langword="false"/> if not</returns>
        bool TryGetDriverType(WebDriverCreationOptions options, IConfigurationSection configuration, out Type driverType)
        {
            driverType = null;
            if(options.DriverFactoryType != null)
                return true;
                        
            if(options.DriverType is null)
            {
                logger.LogError("{ParamName} is mandatory unless {FactoryTypeKey} is specified; the configuration '{ConfigKey}' will be omitted.",
                                nameof(WebDriverCreationOptions.DriverType),
                                nameof(WebDriverCreationOptions.DriverFactoryType),
                                configuration.Key);
                return false;
            }

            try
            {
                driverType = typeProvider.GetWebDriverType(options.DriverType);
                return true;
            }
            catch(Exception e)
            {
                logger.LogError(e,
                                "No implementation of {WebDriverIface} could be found for the {DriverTypeProp} '{DriverType}'; the driver configuration '{ConfigKey}' will be omitted. " +
                                "Reminder: If the driver type is not one which is shipped with Selenium then you must specify its assembly-qualified type name.",
                                nameof(IWebDriver),
                                nameof(WebDriverCreationOptions.DriverType),
                                options.DriverType,
                                configuration.Key);
                return false;
            }
        }

        /// <summary>
        /// Validates and gets the <see cref="Type"/> of the implementation of <see cref="DriverOptions"/> implementation indicated by the configuration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note that it is valid for the options type to be <see langword="null"/> if <see cref="WebDriverCreationOptions.DriverFactoryType"/> is specified.
        /// In that scenario, the options type is unused, but it still indicates a valid configuration.
        /// </para>
        /// </remarks>
        /// <param name="options">The options, as they have been parsed so far</param>
        /// <param name="configuration">The configuration section</param>
        /// <param name="driverType">The type of the Web Driver, as has already been determined by
        /// <see cref="TryGetDriverType(WebDriverCreationOptions, IConfigurationSection, out Type)"/>.</param>
        /// <param name="optionsType">If this method returns <see langword="true"/> then this is a <see cref="Type"/> of the driver options, otherwise
        /// this value is undefined and must be ignored.</param>
        /// <returns><see langword="true"/> if the driver type information is valid; <see langword="false"/> if not</returns>
        bool TryGetOptionsType(WebDriverCreationOptions options, IConfigurationSection configuration, Type driverType, out Type optionsType)
        {
            optionsType = null;
            if(options.DriverFactoryType != null)
                return true;
            
            try
            {
                optionsType = typeProvider.GetWebDriverOptionsType(driverType, options.OptionsType);                
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
                                options.OptionsType,
                                configuration.Key);
                return false;
            }

            try
            {
                options.OptionsFactory = GetOptions(optionsType, configuration);
                return true;
            }
            catch(Exception e)
            {
                logger.LogError(e,
                                "An unexpected error occurred creating or binding to the {OptionsClass} type {OptionsType}; the configuration '{ConfigKey}' will be omitted.",
                                nameof(DriverOptions),
                                optionsType.FullName,
                                configuration.Key);
                return false;
            }
        }

        bool TrySetOptionsCustomizer(WebDriverCreationOptions options, IConfigurationSection configuration, Type optionsType)
        {
            var customizerTypeName = configuration.GetValue<string>("OptionsCustomizerType");
            try
            {
                options.OptionsCustomizer = GetOptionsCustomizer(optionsType, customizerTypeName);
                return true;
            }
            catch(Exception e)
            {
                logger.LogError(e,
                                "An unexpected error occurred binding the {OptionsCustomizer} type {CustomizerType}; the configuration '{ConfigKey}' will be omitted.",
                                nameof(WebDriverCreationOptions.OptionsCustomizer),
                                customizerTypeName,
                                configuration.Key);
                return false;
            }
        }

        static Func<DriverOptions> GetOptions(Type optionsType, IConfigurationSection config)
        {
            return () =>
            {
                var options = (DriverOptions)Activator.CreateInstance(optionsType);
                config.Bind("Options", options);
                return options;
            };
        }

        static object GetOptionsCustomizer(Type optionsType, string customizerTypeName)
        {
            if(string.IsNullOrWhiteSpace(customizerTypeName)) return null;
            var customizerType = Type.GetType(customizerTypeName, true);

            if(!typeof(ICustomizesOptions<>).MakeGenericType(optionsType).IsAssignableFrom(customizerType))
                throw new ArgumentException($"The specified customizer type must implement {nameof(ICustomizesOptions<DriverOptions>)}<{optionsType.Name}>.", nameof(customizerTypeName));
            if(customizerType.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException($"The specified customizer type must have a public parameterless constructor.", nameof(customizerTypeName));
            
            return Activator.CreateInstance(customizerType);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverConfigurationItemParser"/> class.
        /// </summary>
        /// <param name="typeProvider">The provider for web driver and options types.</param>
        /// <param name="logger">The logger for this parser.</param>
        public WebDriverConfigurationItemParser(IGetsWebDriverAndOptionsTypes typeProvider,
                                                ILogger<WebDriverConfigurationItemParser> logger)
        {
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}

