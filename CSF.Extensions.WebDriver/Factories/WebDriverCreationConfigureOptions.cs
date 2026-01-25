using System;
using System.Collections.Generic;
using System.IO;
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
        readonly IParsesSingleWebDriverConfigurationSection configParser;
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

        IDictionary<string, WebDriverCreationOptions> GetDriverConfigurations(IConfigurationSection configuration)
            => configuration.GetChildren()
                            .Select(c => new { c.Key, Value = configParser.GetDriverConfiguration(c) })
                            .Where(x => x.Value != null)
                            .ToDictionary(k => k.Key, v => v.Value);

        

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverCreationConfigureOptions"/>.
        /// </summary>
        /// <param name="configParser">A parser for a single configuration item.</param>
        /// <param name="configuration">The app configuration.</param>
        /// <param name="logger">A logging implementation.</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverCreationConfigureOptions(IParsesSingleWebDriverConfigurationSection configParser,
                                                 IConfiguration configuration,
                                                 ILogger<WebDriverCreationConfigureOptions> logger)
        {
            this.configParser = configParser ?? throw new ArgumentNullException(nameof(configParser));
            this.configuration = configuration;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}