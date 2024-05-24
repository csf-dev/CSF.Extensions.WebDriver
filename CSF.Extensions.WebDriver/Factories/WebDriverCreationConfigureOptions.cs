using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    public class WebDriverCreationConfigureOptions : IConfigureOptions<WebDriverCreationOptionsCollection>
    {
        readonly IConfiguration configuration;
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
            var configurationSection = configuration.GetSection("WebDriverFactory");
            BindOptions(options, configurationSection);
        }

    
        void BindOptions(WebDriverCreationOptionsCollection options, IConfiguration config)
        {
            options.SelectedConfiguration = config.GetValue<string>(nameof(WebDriverCreationOptionsCollection.SelectedConfiguration));

            var driverConfigsSection = config.GetSection(nameof(WebDriverCreationOptionsCollection.DriverConfigurations));
            if(driverConfigsSection != null) options.DriverConfigurations = GetDriverConfigurations(driverConfigsSection);
        }

        IDictionary<string, WebDriverCreationOptions> GetDriverConfigurations(IConfigurationSection configuration)
            => configuration.GetChildren().Select(c => new { c.Key, Value = GetDriverConfiguration(c) }).ToDictionary(k => k.Key, v => v.Value);

        WebDriverCreationOptions GetDriverConfiguration(IConfigurationSection configuration)
        {
            var output = new WebDriverCreationOptions
            {
                DriverType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.DriverType)),
                OptionsType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.OptionsType)),
                GridUrl = configuration.GetValue<string>(nameof(WebDriverCreationOptions.GridUrl)),
                DriverFactoryType = configuration.GetValue<string>(nameof(WebDriverCreationOptions.DriverFactoryType)),
            };

            var driverType = GetDriverType(output.DriverType);
            if (driverType is null)
                throw new ArgumentException($"No implementation of {nameof(IWebDriver)} can be found matching the configured value '{output.DriverType}'.\n" +
                                            "Reminder: If the driver type is not one which is shipped with Selenium then you must specify the assembly-qualified type name.",
                                            nameof(configuration));
            
            
            var driversRootSection = configuration.GetSection(nameof(WebDriverCreationOptionsCollection.DriverConfigurations));
            if (driversRootSection is null) return new WebDriverCreationOptions();
            var driverSections = driversRootSection.GetChildren();

            throw new NotImplementedException();
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
            if(driverTypeName is null) throw new ArgumentNullException(nameof(driverTypeName), $"{nameof(WebDriverCreationOptions.DriverType)} is a mandatory parameter; it must not be null.");
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
                    throw new ArgumentException($"The type specified in {nameof(WebDriverCreationOptions.OptionsType)}: '{optionsTypeName}' could not be loaded.",
                                                nameof(optionsTypeName),
                                                e);
                }
            }

            if (!deterministicOptionsTypes.TryGetValue(driverType, out var typePair))
                throw new ArgumentException($"{nameof(WebDriverCreationOptions.OptionsType)} may only be omitted when the {nameof(WebDriverCreationOptions.DriverType)} " +
                                            $"indicates a local {nameof(IWebDriver)} implementation which implies a deterministic options type in its constructor.  " +
                                            $"For {driverType.FullName}, this is not the case.",
                                            nameof(optionsTypeName));

            return typePair.OptionsType;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverCreationConfigureOptions"/>.
        /// </summary>
        /// <param name="deterministicWebDriverTypesScanner">The deterministic types scanner.</param>
        /// <param name="configuration">The app configuration.</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverCreationConfigureOptions(IGetsWebDriverWithDeterministicOptionsTypes deterministicWebDriverTypesScanner,
                                                 IConfiguration configuration)
        {
            this.deterministicWebDriverTypesScanner = deterministicWebDriverTypesScanner ?? throw new ArgumentNullException(nameof(deterministicWebDriverTypesScanner));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}