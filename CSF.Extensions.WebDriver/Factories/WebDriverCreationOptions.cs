using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Describes the implementation and options for the creation of a web driver.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For most local WebDriver scenarios, the only mandatory properties for this object are <see cref="DriverType"/>
    /// and <see cref="Options"/>.
    /// For remote WebDrivers, the <see cref="OptionsType"/> property is mandatory and the <see cref="GridUrl"/> property is recommended.
    /// </para>
    /// </remarks>
    public class WebDriverCreationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating the concrete <see cref="Type"/> name of the class which should be used
        /// as the <see cref="IWebDriver"/> implementation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property value is mandatory on all instances of web driver configuration.
        /// The type indicated here must derive from <see cref="IWebDriver"/>.
        /// </para>
        /// <para>
        /// For WebDriver implementations which are shipped with Selenium, all that is required is the
        /// simple type name.
        /// For WebDriver implementations that are not part of the <c>Selenium.WebDriver</c> NuGet package,
        /// this should be an assembly-qualified type name, such that the type could be located with
        /// <see cref="Type.GetType(string)"/>.
        /// </para>
        /// <para>
        /// When this value is either <c>RemoteWebDriver</c>, or when it is set to a WebDriver which is not part
        /// of the <c>Selenium.WebDriver</c> NuGet package, then <see cref="OptionsType"/> must also be set.
        /// For local drivers which are shipped with Selenium, explicitly setting the options type is not neccesary;
        /// this library will automatically select the appropriate type.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// To use the Google Chrome implementation of <see cref="IWebDriver"/>, shipped with Selenium, this property should be
        /// set to the value <c>ChromeDriver</c>.
        /// </para>
        /// <para>
        /// To use a (fictitious) WebDriver named ElephantDriver, from an assembly named PachydermWeb.ElephantBrowser, which
        /// has the same namespace, then this property value should be set to <c>PachydermWeb.ElephantBrowser.ElephantDriver, PachydermWeb.ElephantBrowser</c>.
        /// </para>
        /// </example>
        /// <seealso cref="OptionsType"/>
        public string DriverType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the concrete <see cref="Type"/> of the options object which should be provded
        /// to the WebDriver when the driver is created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value only needs to be provided in two scenarios:
        /// </para>
        /// <list type="bullet">
        /// <item><description>When the <see cref="DriverType"/> is set to <c>RemoteWebDriver</c></description></item>
        /// <item><description>When the <see cref="DriverType"/> is set to a driver implementation which is not
        /// shipped with Selenium in the <c>Selenium.WebDriver</c> NuGet package</description></item>
        /// </list>
        /// <para>
        /// For local WebDriver implementations which are shipped with Selenium, this library will automatically select and use
        /// the appropriate options type if this property is <see langword="null" />.
        /// </para>
        /// <para>
        /// In a similar manner to <see cref="DriverType"/>, if 
        /// </para>
        /// </remarks>
        /// <seealso cref="DriverType"/>
        public string OptionsType { get; set; }

        /// <summary>
        /// Applicable to remote web drivers only, gets or sets the URL at which the Selenium Grid is hosted.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this property is unused and irrelevant if <see cref="DriverType"/> is not set to
        /// <c>RemoteWebDriver</c>.
        /// </para>
        /// <para>
        /// When using a remote web driver, it is usually required to set this property value. The only time a value is not
        /// required is if your Selenium Grid configuration is occupying the default URL, which is unlikely in a production
        /// configuration.
        /// </para>
        /// </remarks>
        public string GridUrl { get; set; }

        /// <summary>
        /// Gets or sets the WebDriver options object which should be provided to the WebDriver implementation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This must be an object of an appropriate type to match the implementation of <see cref="IWebDriver"/> that is selected,
        /// via <see cref="DriverType"/>.
        /// When deserializing this value from configuration (such as an <c>appsettings.json</c> file), the <see cref="OptionsType"/>
        /// will be used to select the appropriate polymorphic type to which the configuration should be bound.
        /// For local WebDriver implementations which are shipped with Selenium, the options type need not be specified explicitly;
        /// it will be inferred from the chosen driver type.
        /// </para>
        /// </remarks>
        public DriverOptions Options { get; set; }

        /// <summary>
        /// Unneeded except in unusual circumstances, gets or sets the name of a type which is used to construct the WebDriver instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is unneeded and should be set to <see langword="null" /> in almost all circumstances.
        /// For all WebDriver implementations bundled with Selenium and most expected third party implementations, this library can
        /// automatically instantiate them without additional logic.
        /// This library can correctly instantiate all WebDrivers that are included with the <c>Selenium.WebDriver</c> NuGet package.
        /// This library can also automatically instantiate any third-party WebDriver implementation so long as it has a public
        /// constructor which takes a single parameter, which is of a type that derives from <see cref="DriverOptions"/>.
        /// </para>
        /// <para>
        /// If <see cref="DriverType"/> is set to a third-party WebDriver implementation which does not follow the pattern above,
        /// such as one which has a different constructor parameter signature, this library
        /// requires some help in instantiating the WebDriver.
        /// In that case (only), this property should be set to the assembly-qualified type name of a type which implements
        /// <see cref="ICreatesWebDriverFromOptions"/>.  That type must be provided by the developer using this library.  The factory
        /// is responsible for constructing the WebDriver instance from options and returning it.
        /// </para>
        /// <para>
        /// When using a factory of this kind, the factory class which implements <see cref="ICreatesWebDriverFromOptions"/> must either:
        /// </para>
        /// <list type="bullet">
        /// <item><description>Be available through dependency injection; add it to your service collection</description></item>
        /// <item><description>Have a public parameterless constructor, such that it may be created via <see cref="Activator.CreateInstance(Type)"/></description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="DriverType"/>
        /// <seealso cref="ICreatesWebDriverFromOptions"/>
        public string DriverFactoryType { get; set; }

        /// <summary>
        /// Gets a value which indicates whether or not the created WebDriver should be enriched with browser identification functionality.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this property is set to <see langword="true"/> (which is the default if left unset) then the implementation of
        /// <see cref="IWebDriver"/> returned by a factory which uses this object as a parameter will be enriched and will implement
        /// <see cref="Identification.IHasBrowserId"/> in addition to its usual interfaces.
        /// </para>
        /// <para>
        /// This functionality, if enabled, will mean that the WebDriver returned by the factory will be a proxy object and not the
        /// original WebDriver implementation.
        /// For more information on proxies created by this library and their implications, see <see cref="IGetsProxyWebDriver"/>.
        /// </para>
        /// </remarks>
        public bool AddBrowserIdentification { get; set; } = true;
    }
}