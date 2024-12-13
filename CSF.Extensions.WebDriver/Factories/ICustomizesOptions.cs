using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// An object which can customize the options for a web driver before they are used to create the <see cref="IWebDriver"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations of this interface are rarely required. They are used to customize the options for creating a web driver
    /// after <see cref="WebDriverCreationOptions.OptionsFactory"/> has created the options instance but before it is used to
    /// create the web driver.
    /// </para>
    /// <para>
    /// This is useful when you need to customize the options for a web driver in a way which is not supported by the binding from
    /// a configuration file.  For example, some web driver options do not provide property getters/setters but must be configured
    /// using methods.  In this case you can implement this interface with a class to customize the options as required.
    /// </para>
    /// <para>
    /// The implementation of this interface should be specified via the <see cref="WebDriverCreationOptions.OptionsCustomizer"/>
    /// property.  This will instantiate the customizer and call the <see cref="CustomizeOptions"/> method before the web driver is
    /// created.
    /// </para>
    /// </remarks>
    public interface ICustomizesOptions<in TOptions> where TOptions : DriverOptions
    {
        /// <summary>
        /// Customizes the options for a web driver.
        /// </summary>
        /// <param name="options">The WebDriver options.</param>
        void CustomizeOptions(TOptions options);
    }
}
