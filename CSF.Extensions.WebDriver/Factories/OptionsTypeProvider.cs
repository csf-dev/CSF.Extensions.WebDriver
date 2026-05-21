using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Default implementation of <see cref="IGetsOptionsType"/>.
    /// </summary>
    public class OptionsTypeProvider : IGetsOptionsType
    {
        readonly ILogger<OptionsTypeProvider> logger;
        readonly IGetsWebDriverAndOptionsTypes typeProvider;
        readonly ICreatesDriverOptions optionsFactory;

        /// <inheritdoc/>
        public bool TryGetOptionsType(WebDriverCreationOptions options, IConfigurationSection configuration, Type driverType, out Type optionsType)
        {
            optionsType = null;
            
            try
            {
                optionsType = typeProvider.GetWebDriverOptionsType(driverType, options.OptionsType);                
            }
            catch(Exception e)
            {
                if(options.DriverFactoryType == null)
                    logger.LogError(e,
                                    "No type deriving from {OptionsBase} could be found for the combination of {WebDriverIface} {DriverType} and {OptionsTypeProp} '{OptionsType}'; the configuration '{ConfigKey}' will be omitted. " +
                                    "See the exception details for more information.",
                                    nameof(DriverOptions),
                                    nameof(IWebDriver),
                                    driverType?.Name,
                                    nameof(WebDriverCreationOptions.OptionsType),
                                    options.OptionsType,
                                    configuration.Key);

                return options.DriverFactoryType != null;
            }

            try
            {
                options.OptionsFactory = GetOptions(optionsType, configuration);
                return true;
            }
            catch(Exception e)
            {
                if(options.DriverFactoryType == null)
                    logger.LogError(e,
                                    "An unexpected error occurred creating or binding to the {OptionsClass} type {OptionsType}; the configuration '{ConfigKey}' will be omitted.",
                                    nameof(DriverOptions),
                                    optionsType.FullName,
                                    configuration.Key);
                return options.DriverFactoryType != null;
            }
        }

        Func<DriverOptions> GetOptions(Type optionsType, IConfigurationSection config)
            => () => optionsFactory.CreateOptions(optionsType, config);

        /// <summary>
        /// Initialises a new instance of <see cref="DriverTypeProvider"/>.
        /// </summary>
        /// <param name="logger">A logger</param>
        /// <param name="typeProvider">A provider for the concrete types of web driver and options</param>
        /// <param name="optionsFactory">A factory for instances of <see cref="DriverOptions"/></param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null"/></exception>
        public OptionsTypeProvider(ILogger<OptionsTypeProvider> logger, IGetsWebDriverAndOptionsTypes typeProvider, ICreatesDriverOptions optionsFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            this.optionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
        }
    }
}

