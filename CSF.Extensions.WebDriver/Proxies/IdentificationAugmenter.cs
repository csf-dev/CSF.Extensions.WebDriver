using System;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Identification;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Implementation of <see cref="IAugmentsProxyContext"/> which adds <see cref="IHasBrowserId"/> to the proxy.
    /// </summary>
    public class IdentificationAugmenter : IAugmentsProxyContext
    {
        readonly IGetsBrowserIdFromWebDriver browserIdFactory;

        /// <inheritdoc/>
        public void AugmentContext(WebDriverProxyCreationContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            if (!context.CreationOptions.AddIdentification) return;

            context.Interfaces.Add(typeof(IHasBrowserId));
            var browserId = browserIdFactory.GetBrowserId(context.WebDriver, context.CreationOptions.DriverOptions);
            var interceptor = new IdentificationInterceptor(browserId);
            context.Interceptors.Add(interceptor);
        }

        public IdentificationAugmenter(IGetsBrowserIdFromWebDriver browserIdFactory)
        {
            this.browserIdFactory = browserIdFactory ?? throw new ArgumentNullException(nameof(browserIdFactory));
        }
    }

    /// <summary>
    /// Interceptor which provides functionality for <see cref="IHasBrowserId"/>.
    /// </summary>
    public class IdentificationInterceptor : IInterceptor
    {
        readonly BrowserId browserId;

        /// <inheritdoc/>
        public void Intercept(IInvocation invocation)
        {
            if (!Is.Getter<IHasBrowserId>(nameof(IHasBrowserId.BrowserId), invocation.Method))
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = browserId;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="IdentificationInterceptor"/>.
        /// </summary>
        /// <param name="browserId">The browser identity</param>
        /// <exception cref="ArgumentNullException">If <paramref name="browserId"/> is <see langword="null" />.</exception>
        public IdentificationInterceptor(BrowserId browserId)
        {
            this.browserId = browserId ?? throw new ArgumentNullException(nameof(browserId));
        }
    }
}