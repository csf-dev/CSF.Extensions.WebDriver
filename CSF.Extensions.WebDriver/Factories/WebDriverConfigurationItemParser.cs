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
        readonly IGetsDriverType driverTypeProvider;
        readonly IGetsOptionsType optionsTypeProvider;
        readonly ICreatesDriverOptions optionsFactory;
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

            if(!driverTypeProvider.TryGetDriverType(creationOptions, configuration, out var driverType))
                return null;

            if(!optionsTypeProvider.TryGetOptionsType(creationOptions, configuration, driverType, out var optionsType))
                return null;

            creationOptions.OptionsFactory = () => optionsFactory.CreateOptions(optionsType, configuration);

            if(!TrySetOptionsCustomizer(creationOptions, configuration, optionsType))
                return null;

            return creationOptions;
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
        /// <param name="driverTypeProvider">A service to get the driver type</param>
        /// <param name="optionsTypeProvider">A service to get the options type</param>
        /// <param name="optionsFactory">A service to get the driver options</param>
        /// <param name="logger">The logger for this parser.</param>
        public WebDriverConfigurationItemParser(IGetsDriverType driverTypeProvider,
                                                IGetsOptionsType optionsTypeProvider,
                                                ICreatesDriverOptions optionsFactory,
                                                ILogger<WebDriverConfigurationItemParser> logger)
        {
            this.driverTypeProvider = driverTypeProvider ?? throw new ArgumentNullException(nameof(driverTypeProvider));
            this.optionsTypeProvider = optionsTypeProvider ?? throw new ArgumentNullException(nameof(optionsTypeProvider));
            this.optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}

