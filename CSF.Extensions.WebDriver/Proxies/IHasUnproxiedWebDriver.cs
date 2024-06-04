using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// An object which provides access to an 'unproxied' <see cref="IWebDriver"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For more information about the proxying functionality within this library and its consequences, see
    /// <see cref="IGetsProxyWebDriver"/>.
    /// </para>
    /// <para>
    /// This interface provides functionality to get the original 'unproxied' WebDriver instance, undoing the
    /// proxying process.  Note that such an unproxied WebDriver will have lost the augmented functionality provided
    /// by this library.
    /// There is also a 'convenience' extension method: <see cref="WebDriverExtensions.Unproxy(IWebDriver)"/> to
    /// use this in a consistent manner.
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