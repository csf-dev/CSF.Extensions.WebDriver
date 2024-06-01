using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Identification;
using CSF.Extensions.WebDriver.Quirks;
using Microsoft.Extensions.DependencyInjection;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Implementation of <see cref="IAugmentsProxyContext"/> which adds <see cref="IHasQuirks"/> to the proxy.
    /// </summary>
    public class QuirksAugmenter : IAugmentsProxyContext
    {
        readonly IServiceProvider services;

        /// <inheritdoc/>
        public void AugmentContext(WebDriverProxyCreationContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            if (!context.CreationOptions.AddQuirks) return;

            context.Interfaces.Add(typeof(IHasQuirks));
            context.Interceptors.Add(ActivatorUtilities.CreateInstance<QuirksInterceptor>(services, new [] {context.BrowserId}));
        }

        /// <summary>
        /// Initialises a new instance of <see cref="QuirksAugmenter"/>.
        /// </summary>
        /// <param name="services">The DI services provider.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="services"/> is <see langword="null" />.</exception>
        public QuirksAugmenter(IServiceProvider services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }

    /// <summary>
    /// Interceptor which provides functionality for <see cref="IHasQuirks"/>.
    /// </summary>
    public class QuirksInterceptor : IInterceptor
    {
        readonly IReadOnlyCollection<string> quirksForBrowser;

        /// <inheritdoc/>
        public void Intercept(IInvocation invocation)
        {
            if (Is.Getter<IHasQuirks>(nameof(IHasQuirks.AllQuirks), invocation.Method))
            {
                invocation.ReturnValue = quirksForBrowser;
                return;
            }

            invocation.Proceed();
        }

        /// <summary>
        /// Initialises a new instance of <see cref="QuirksInterceptor"/>.
        /// </summary>
        /// <param name="browserId">The browser identity</param>
        /// <param name="quirksMatcher">A service to match a browser with its quirks</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        public QuirksInterceptor(BrowserId browserId, IGetsQuirksForBrowserId quirksMatcher)
        {
            if (browserId is null) throw new ArgumentNullException(nameof(browserId));
            if (quirksMatcher is null) throw new ArgumentNullException(nameof(quirksMatcher));

            quirksForBrowser = quirksMatcher.GetApplicableQuirks(browserId);
        }
    }
}