using System;
using System.Collections.Generic;
using System.Linq;
using CSF.Extensions.WebDriver.Identification;
using CSF.Extensions.WebDriver.Proxies;
using CSF.Extensions.WebDriver.Quirks;

namespace OpenQA.Selenium
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
        /// <para>
        /// For more information about this library's proxying functionality, see <see cref="IGetsProxyWebDriver"/>.
        /// </para>
        /// </remarks>
        /// <param name="webDriver">A WebDriver which may or may not be a proxy.</param>
        /// <returns>An 'unproxied' WebDriver</returns>
        /// <seealso cref="IHasUnproxiedWebDriver"/>
        /// <seealso cref="IGetsProxyWebDriver"/>
        public static IWebDriver Unproxy(this IWebDriver webDriver)
            => webDriver is IHasUnproxiedWebDriver proxy ? proxy.UnproxiedWebDriver : webDriver;

        /// <summary>
        /// Gets the <see cref="BrowserId"/> from a WebDriver, if the browser identification functionality is activated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the WebDriver is not a proxy which was created via <see cref="IGetsProxyWebDriver"/>, or if both
        /// <see cref="ProxyCreationOptions.AddIdentification"/> &amp; <see cref="ProxyCreationOptions.AddQuirks"/>
        /// were <see langword="false" /> when the proxy was created then this method will return <see langword="null" />.
        /// Otherwise, this method will get the browser identification information from the proxy.
        /// </para>
        /// <para>
        /// This is a convenience method which avoids the need to manually type-check for the <see cref="IHasBrowserId"/>
        /// interface.
        /// </para>
        /// </remarks>
        /// <returns>The browser identity information, or a <see langword="null" /> reference.</returns>
        public static BrowserId GetBrowserId(this IWebDriver webDriver)
            => webDriver is IHasBrowserId browserId ? browserId.BrowserId : null;

        /// <summary>
        /// Gets a collection of string quirk-names which affect the specified WebDriver, if the browser quirks
        /// functionality is activated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the WebDriver is not a proxy which was created via <see cref="IGetsProxyWebDriver"/>, or if 
        /// <see cref="ProxyCreationOptions.AddQuirks"/> was <see langword="false" /> when the proxy was created
        /// then this method will return an empty collection.
        /// Otherwise, this method will get the collection of named quirks which affect the current WebDriver.
        /// </para>
        /// <para>
        /// This is a convenience method which avoids the need to manually type-check for the <see cref="IHasQuirks"/>
        /// interface.
        /// </para>
        /// <para>
        /// For more information on browser quirks and their purpose, please read the remarks for <see cref="IHasQuirks"/>.
        /// </para>
        /// </remarks>
        /// <returns>The browser quirks, or an empty collection.</returns>
        public static IReadOnlyCollection<string> GetQuirks(this IWebDriver webDriver)
            => webDriver is IHasQuirks quirks ? quirks.AllQuirks : Array.Empty<string>();

        /// <summary>
        /// Gets a value indicating whether or not the current object is affected by the specified named quirk.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the WebDriver is not a proxy which was created via <see cref="IGetsProxyWebDriver"/>, or if 
        /// <see cref="ProxyCreationOptions.AddQuirks"/> was <see langword="false" /> when the proxy was created
        /// then this method will always return <see langword="false" />.
        /// Otherwise, this method will return <see langword="true" /> only if the specified WebDriver is affected by
        /// the specified quirk.
        /// </para>
        /// <para>
        /// For more information on what quirks are, see the documentation for <see cref="IHasQuirks"/>.
        /// </para>
        /// </remarks>
        /// <param name="webDriver">A WebDriver</param>
        /// <param name="quirkName">The name of a quirk</param>
        /// <returns><see langword="true" /> if the WebDriver is affected by the specified quirk; <see langword="false" /> otherwise.</returns>
        public static bool HasQuirk(this IWebDriver webDriver, string quirkName)
        {
            if (webDriver is null) throw new ArgumentNullException(nameof(webDriver));
            if (string.IsNullOrEmpty(quirkName))
                throw new ArgumentException("The quirk name must not be null or an empty string", "quirkName");

            return webDriver.GetQuirks().Contains(quirkName);
        }

        /// <summary>
        /// Gets the first of an ordered collection of quirks which affects the specified WebDriver.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the WebDriver is not a proxy which was created via <see cref="IGetsProxyWebDriver"/>, or if 
        /// <see cref="ProxyCreationOptions.AddQuirks"/> was <see langword="false" /> when the proxy was created
        /// then this method will always return <see langword="null" />.
        /// Otherwise, if the WebDriver is affected by any of the specified quirks then this method will return
        /// the string quirk-name of the first of the <paramref name="orderedQuirks"/> which is applicable to the
        /// WebDriver.  It will return <see langword="null" /> if none of the specified quirks are applicable to
        /// the WebDriver.
        /// </para>
        /// <para>
        /// For more information on what quirks are, see the documentation for <see cref="IHasQuirks"/>.
        /// </para>
        /// </remarks>
        /// <param name="webDriver">A WebDriver</param>
        /// <param name="orderedQuirks">The an ordered collection of quirk names</param>
        /// <returns>The first item from <paramref name="orderedQuirks"/> which is applicable to the WebDriver, or a
        /// <see langword="null" /> reference if none of the specified quirks are applicable.</returns>
        public static string GetFirstApplicableQuirk(this IWebDriver webDriver, params string[] orderedQuirks)
        {
            if (webDriver is null) throw new ArgumentNullException(nameof(webDriver));
            if (orderedQuirks is null || orderedQuirks.Any(string.IsNullOrEmpty))
                throw new ArgumentException("The collection of quirks must not be null or contain any null or empty-string values.", nameof(orderedQuirks));

            return orderedQuirks.FirstOrDefault(x => webDriver.HasQuirk(x));
        }
    }
}