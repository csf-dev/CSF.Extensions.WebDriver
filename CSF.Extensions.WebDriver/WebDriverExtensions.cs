using CSF.Extensions.WebDriver.Proxies;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver
{
    /// <summary>
    /// Extension methods for <see cref="IWebDriver"/>.
    /// </summary>
    public static class WebDriverExtensions
    {
        /// <summary>
        /// 'Unproxies' a WebDriver which may or may not be a proxy.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <paramref name="webDriver"/> is a proxy then this method returns the original 'unproxied' driver.
        /// If the WebDriver is not a proxy then this method returns the <paramref name="webDriver"/> directly.
        /// This way, whether the input parameter was a proxy or not, the output is an unproxied WebDriver.
        /// </para>
        /// </remarks>
        /// <param name="webDriver">A WebDriver which may or may not be a proxy.</param>
        /// <returns>An 'unproxied' WebDriver</returns>
        /// <seealso cref="IHasUnproxiedWebDriver"/>
        public static IWebDriver Unproxy(this IWebDriver webDriver)
            => webDriver is IHasUnproxiedWebDriver proxy ? proxy.UnproxiedWebDriver : webDriver;
    }
}