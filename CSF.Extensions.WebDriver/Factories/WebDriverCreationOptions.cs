using System;
using CSF.Extensions.WebDriver.Proxies;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Describes the implementation and options for the creation of a web driver.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For most local WebDriver scenarios, the only mandatory properties for this object are <see cref="DriverType"/>
    /// and <see cref="OptionsFactory"/>.
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
        /// In a similar manner to <see cref="DriverType"/>, if this property is set to an options type which is shipped with Selenium
        /// out-of-the-box then it need only be set to the simple type name.  If a custom options type is required, which does not ship
        /// with Selenium, then this must be an assembly-qualified type name.
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
        /// Gets or sets a function which creates the object which derives from <see cref="DriverOptions"/>, used as the creation options for the <see cref="IWebDriver"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the most common scenario - providing WebDriver options from a JSON configuration file such as <c>appsettings.json</c> - this property is bound
        /// from a configuration key named <c>Options</c>, rather than "OptionsFactory".  In a configuration file, the options are specified as a simple
        /// JSON object. However, after binding to this property this becomes a factory function instead.  That is because of two factors:
        /// </para>
        /// <list type="bullet">
        /// <item><description>Instances of types which derive from <see cref="DriverOptions"/> are not reusable and should not be shared between WebDriver instances</description></item>
        /// <item><description>This WebDriver factory framework must be capable of creating multiple <see cref="IWebDriver"/> instances from one configuration, thus
        /// requiring many options instances</description></item>
        /// When bound from a configuration file, the options object which would be returned from this factory function will have properties set
        /// as specified in that configuration.
        /// </list>
        /// <para>
        /// The return value of this function must be an object of an appropriate type to match the implementation of <see cref="IWebDriver"/>
        /// that is selected, via <see cref="DriverType"/>.
        /// If this value was bound from a configuration file then the generated factory function will automatically instantiate an instance of either:
        /// </para>
        /// <list type="bullet">
        /// <item><description>The options type specified in the configuration file, if <see cref="OptionsType"/> is set</description></item>
        /// <item><description>The options type which is inferred from the <see cref="DriverType"/>, if <see cref="OptionsType"/> is not set.
        /// See the documentation for <see cref="OptionsType"/> for more information</description></item>
        /// </list>
        /// </remarks>
        public Func<DriverOptions> OptionsFactory { get; set; }

        /// <summary>
        /// An optional object which implements <see cref="ICustomizesOptions{TOptions}"/> for the corresponding <see cref="DriverOptions"/>
        /// type for the <see cref="DriverType"/>/<see cref="OptionsType"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this instance is bound from a configuration file - such as <c>appsettings.json</c> - then this property is bound from a configuration key named
        /// <c>OptionsCustomizerType</c> rather than "OptionsCustomizer".  The value of that configuration key should be the assembly-qualified type name of
        /// the concrete implementation of <see cref="ICustomizesOptions{TOptions}"/> which should be used to customize the options. In this scenario this type
        /// must also have a public parameterless constructor.
        /// </para>
        /// <para>
        /// This configuration property is rarely required. This object is used to customize the options for creating a web driver
        /// after <see cref="OptionsFactory"/> has created the options instance but before it is used to
        /// create the web driver.
        /// </para>
        /// <para>
        /// This is useful when you need to customize the options for a web driver in a way which is not supported by the binding from
        /// a configuration file.  For example, some web driver options do not provide property getters/setters but must be configured
        /// using methods.  In this case you can implement this interface with a class to customize the options as required.
        /// </para>
        /// </remarks>
        public object OptionsCustomizer { get; set; }

        /// <summary>
        /// Gets or sets the name of a type which is used to construct the WebDriver instance; unneeded except in unusual circumstances.
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
        /// </para>
        /// <para>
        /// Alternatively, you may need to use a custom factory, identified by this configuration property, if you require additional customisation
        /// of the WebDriver after creation, which cannot be achieved only with options.
        /// For example, if you are making use a custom third-party integration which adds an additional layer of capabilities to
        /// communicate the name of a currently-running test.  This information can be retrieved only at runtime and not from configuration.
        /// </para>
        /// <para>
        /// In cases like the above, this property should be set to the assembly-qualified type name of a type which implements
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

        /// <summary>
        /// Gets a value which indicates whether or not the created WebDriver should be enriched with browser quirks functionality.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For more information about this functionality, please see the documentation for <see cref="Quirks.IHasQuirks"/>
        /// </para>
        /// <para>
        /// This functionality, if enabled, will mean that the WebDriver returned by the factory will be a proxy object and not the
        /// original WebDriver implementation.
        /// For more information on proxies created by this library and their implications, see <see cref="IGetsProxyWebDriver"/>.
        /// </para>
        /// <para>
        /// If this property is <see langword="true" /> then the value of <see cref="AddBrowserIdentification"/> is irrelevant;
        /// browser identification will always be added to the proxy when quirks are added.
        /// </para>
        /// </remarks>
        public bool AddBrowserQuirks { get; set; }
    }
}