using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Default implementation of <see cref="IGetsDriverType"/>.
    /// </summary>
    public class DriverTypeProvider : IGetsDriverType
    {
        readonly ILogger logger;
        readonly IGetsWebDriverAndOptionsTypes typeProvider;

        /// <inheritdoc/>
        public bool TryGetDriverType(WebDriverCreationOptions options, IConfigurationSection configuration, out Type driverType)
        {
            driverType = null;
                        
            if(options.DriverType is null)
            {
                if(options.DriverFactoryType == null)
                    logger.LogError("{ParamName} is mandatory unless {FactoryTypeKey} is specified; the configuration '{ConfigKey}' will be omitted.",
                                    nameof(WebDriverCreationOptions.DriverType),
                                    nameof(WebDriverCreationOptions.DriverFactoryType),
                                    configuration.Key);
                return options.DriverFactoryType != null;
            }

            try
            {
                driverType = typeProvider.GetWebDriverType(options.DriverType);
                return true;
            }
            catch(Exception e)
            {
                if(options.DriverFactoryType == null)
                    logger.LogError(e,
                                    "No implementation of {WebDriverIface} could be found for the {DriverTypeProp} '{DriverType}'; the driver configuration '{ConfigKey}' will be omitted. " +
                                    "Reminder: If the driver type is not one which is shipped with Selenium then you must specify its assembly-qualified type name.",
                                    nameof(IWebDriver),
                                    nameof(WebDriverCreationOptions.DriverType),
                                    options.DriverType,
                                    configuration.Key);
                return options.DriverFactoryType != null;
            }
        }

        /// <summary>
        /// Initialises a new instance of <see cref="DriverTypeProvider"/>.
        /// </summary>
        /// <param name="loggerProvider">A logger provider</param>
        /// <param name="typeProvider">A provider for the concrete types of web driver and options</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null"/></exception>
        public DriverTypeProvider(IGetsLogger loggerProvider, IGetsWebDriverAndOptionsTypes typeProvider)
        {
            if (loggerProvider is null) throw new ArgumentNullException(nameof(loggerProvider));

            logger = loggerProvider.GetLogger<DriverTypeProvider>();
            this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
        }
    }
}

