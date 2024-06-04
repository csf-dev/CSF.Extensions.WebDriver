using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Identification;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// An object which serves as a factory for proxy instances of <see cref="IWebDriver"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some of the functionality of <c>CSF.Extensions.WebDriver</c> requires the replacement of the WebDriver
    /// with a proxy object.  The proxy is an object which implements all of the same interfaces as the original
    /// WebDriver and behaves in the same way but is augmented with additional functionality.
    /// </para>
    /// <para>
    /// This proxying process is presently implemented via Castle DynamicProxy: https://www.castleproject.org/projects/dynamicproxy/.
    /// This kind of augmentation of objects is typically performed via the decorator pattern: https://en.wikipedia.org/wiki/Decorator_pattern.
    /// That pattern is unsuitable here though because it is impossible to know the complete list of interfaces
    /// that the original WebDriver object implements until runtime.  Without proxies this library would be forced to either:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Assume that the WebDriver implements all of Selenium's possible interfaces, possibly claiming to implement
    /// some interfaces that the WebDriver does not support</description></item>
    /// <item><description>Or implement only a minimal set of interfaces: the lowest common denominator, which would result in a less
    /// useful WebDriver object</description></item>
    /// </list>
    /// <para>
    /// In addition, a new third-party WebDriver which implements as-yet-unknown interfaces would not be able to expose them from a
    /// traditional decorator.
    /// </para>
    /// <para>
    /// A proxy, which is created at runtime, may use reflection to determine all of the interfaces implemented by the WebDriver and
    /// implement those same interfaces, whilst adding support for the extra interfaces that are provided by this library.
    /// </para>
    /// <para>
    /// In practice, this has no effect upon the returned instance of <see cref="IWebDriver"/>; any casting/safe-casting logic which
    /// relies on Selenium or other interfaces will continue to work as it has done before.  Logic such as the following is still
    /// possible if <c>webDriverProxy</c> is a proxy object.
    /// </para>
    /// <code>
    /// if(webDriverProxy is IDevTools devToolsDriver)
    /// {
    ///     var session = devToolsDriver.GetDevToolsSession();
    ///     // ...
    /// }
    /// </code>
    /// <para>
    /// What will not work with a proxy object is casting back to the original <see cref="IWebDriver"/> implementation class type.
    /// The following would be <see langword="false" /> if <c>webDriverProxy</c> were a proxy object:
    /// </para>
    /// <code>
    /// var amIChrome = webDriverProxy is ChromeDriver;
    /// </code>
    /// <para>
    /// Using a functionality-providing object via its interfaces and avoiding reliance upon concrete classes is generally best practice
    /// across all of software development so well-written code should not be concerned with this limitation.
    /// To cope with the rare scenarios in which this limitation is troublesome, all WebDriver proxies are enhanced with the interface
    /// <see cref="IHasUnproxiedWebDriver"/>.  This interface provides functionality to get the original 'unproxied' WebDriver instance.
    /// There is also an extension method for the <see cref="IWebDriver"/> interface: <see cref="WebDriverExtensions.Unproxy(IWebDriver)"/>
    /// to provide convenient and reliable access to that functionality.
    /// </para>
    /// </remarks>
    public interface IGetsProxyWebDriver
    {
        /// <summary>
        /// Gets a proxy object implementing <see cref="IWebDriver"/> as well as all of the other interfaces implemented by the
        /// <paramref name="webDriver"/>, enriching it with additional functionality as determined by the <paramref name="options"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See the remarks upon <see cref="IGetsProxyWebDriver"/> for more information.
        /// </para>
        /// </remarks>
        /// <param name="webDriver">The original WebDriver instance to proxy.</param>
        /// <param name="options">Options related to the creation of this proxy.</param>
        /// <returns>A proxy <see cref="IWebDriver"/> instance.</returns>
        IWebDriver GetProxyWebDriver(IWebDriver webDriver, ProxyCreationOptions options);
    }
}