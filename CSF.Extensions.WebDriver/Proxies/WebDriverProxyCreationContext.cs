using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Identification;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Contextual information related to the creation of a WebDriver proxy.
    /// </summary>
    public class WebDriverProxyCreationContext
    {
        static readonly Type[] excludedInterfaces = new[] { typeof(IWebDriver), typeof(IProxyTargetAccessor) };

        /// <summary>
        /// Gets a collection of the interfaces which the proxy object shall implement.
        /// </summary>
        public ISet<Type> Interfaces { get; }

        /// <summary>
        /// Gets a collection of the Castle DynamicProxy interceptors which shall be applied to the proxy.
        /// </summary>
        public ISet<IInterceptor> Interceptors { get; } = new HashSet<IInterceptor>();

        /// <summary>
        /// Gets the WebDriver object from which a proxy will be created.
        /// </summary>
        public IWebDriver WebDriver { get; }

        /// <summary>
        /// Gets or sets the identity information for the browser.
        /// </summary>
        public BrowserId BrowserId { get; set; }

        /// <summary>
        /// Gets the driver options from which the WebDriver was originally created.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Although it should be unlikely, consumers should deal with scenarios in which this property value
        /// is <see langword="null" />.
        /// </para>
        /// </remarks>
        public ProxyCreationOptions CreationOptions { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverProxyCreationContext"/>.
        /// </summary>
        /// <param name="webDriver">The WebDriver from which the proxy will be created.</param>
        /// <param name="creationOptions">The WebDriver proxy creation options.</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverProxyCreationContext(IWebDriver webDriver, ProxyCreationOptions creationOptions)
        {
            WebDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            CreationOptions = creationOptions ?? throw new ArgumentNullException(nameof(creationOptions));
            var interfaces = webDriver.GetType()
                .GetInterfaces()
                .Except(excludedInterfaces);
            Interfaces = new HashSet<Type>(interfaces);
        }
    }
}