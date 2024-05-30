using System;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Factory class which creates a proxy WebDriver using Castle.DynamicProxy.
    /// </summary>
    public class WebDriverProxyFactory : IGetsProxyWebDriver
    {
        /// <summary>
        /// A collection of the types of <see cref="IAugmentsProxyContext"/> augmenters that will be used by this factory.
        /// </summary>
        static readonly Type[] augmenterTypes = {
            typeof(UnproxyingAugmenter),
            typeof(IdentificationAugmenter),
        };

        readonly IServiceProvider services;

        /// <inheritdoc/>
        public IWebDriver GetProxyWebDriver(IWebDriver webDriver, ProxyCreationOptions options)
        {
            if (webDriver is null) throw new ArgumentNullException(nameof(webDriver));
            if (options is null) throw new ArgumentNullException(nameof(options));

            var context = new WebDriverProxyCreationContext(webDriver, options);
            AugmentContext(context);
            return GetProxyFromContext(context);
        }

        /// <summary>
        /// Augments the specified context with additional interfaces and/or interceptors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Essentially, this is where we decide what extra functionality we're going to add to the WebDriver via the proxy.
        /// We get a collection of <see cref="IAugmentsProxyContext"/> implementations, one for each piece of functionality,
        /// then use each of those to add to the context.
        /// </para>
        /// </remarks>
        /// <param name="context">The creation context.</param>
        void AugmentContext(WebDriverProxyCreationContext context)
        {
            var augmenters = augmenterTypes
                .Select(type => services.GetService(type))
                .Cast<IAugmentsProxyContext>()
                .ToList();

            foreach (var augmenter in augmenters)
                augmenter.AugmentContext(context);
        }

        /// <summary>
        /// Creates a WebDriver proxy from the creation context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Now that we have decided how we are going to extend the proxy WebDriver and stored that info
        /// in the context, here we unpack all of the interfaces and interceptors from the context and create
        /// the proxy using Castle DynamicProxy's functionality.
        /// </para>
        /// </remarks>
        /// <param name="context">The creation context</param>
        /// <returns>A proxy WebDriver.</returns>
        IWebDriver GetProxyFromContext(WebDriverProxyCreationContext context)
        {
            var proxyGenerator = services.GetRequiredService<IProxyGenerator>();
            return (IWebDriver) proxyGenerator.CreateInterfaceProxyWithTargetInterface(typeof(IWebDriver),
                                                                                       context.Interfaces.ToArray(),
                                                                                       context.WebDriver,
                                                                                       context.Interceptors.ToArray());
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverProxyFactory"/>.
        /// </summary>
        /// <param name="services">DI services.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="services"/> is <see langword="null" />.</exception>
        public WebDriverProxyFactory(IServiceProvider services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}