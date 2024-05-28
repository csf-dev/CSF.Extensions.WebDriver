using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// An object which provides access to an 'unproxied' <see cref="IWebDriver"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some of the functionality of <c>CSF.Extensions.WebDriver</c> requires the replacement of the WebDriver
    /// with a proxy object.  The proxy is an object which implements all of the same interfaces as the original
    /// WebDriver but also some additional interfaces, providing the extra functionality.
    /// In all common scenarios, this has no impact upon the usage of the WebDriver, because best practice is
    /// to use it via the <see cref="IWebDriver"/> interface, and other related interfaces.
    /// </para>
    /// <para>
    /// It is the case, though, that such a proxy object cannot be cast directly back to its original WebDriver type.
    /// If the original WebDriver is required, then this interface (which is implemented by all proxies) provides
    /// access to the original 'unproxied' WebDriver which is wrapped by the proxy.
    /// </para>
    /// </remarks>
    /// <seealso cref="WebDriverExtensions.Unproxy(IWebDriver)"/>
    public interface IHasUnproxiedWebDriver
    {
        /// <summary>
        /// Gets the 'unproxied' WebDriver.
        /// </summary>
        IWebDriver UnproxiedWebDriver { get; }
    }
}