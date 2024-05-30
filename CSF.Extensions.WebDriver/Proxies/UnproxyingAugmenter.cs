using System;
using Castle.DynamicProxy;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Implementation of <see cref="IAugmentsProxyContext"/> which adds <see cref="IHasUnproxiedWebDriver"/> to the proxy.
    /// </summary>
    public class UnproxyingAugmenter : IAugmentsProxyContext
    {
        /// <inheritdoc/>
        public void AugmentContext(WebDriverProxyCreationContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            context.Interfaces.Add(typeof(IHasUnproxiedWebDriver));
            context.Interceptors.Add(new UnproxyingInterceptor(context.WebDriver));
        }
    }

    /// <summary>
    /// Interceptor which provides functionality for <see cref="IHasUnproxiedWebDriver"/>.
    /// </summary>
    public class UnproxyingInterceptor : IInterceptor
    {
        readonly IWebDriver unproxiedDriver;

        /// <inheritdoc/>
        public void Intercept(IInvocation invocation)
        {
            if (!Is.Getter<IHasUnproxiedWebDriver>(nameof(IHasUnproxiedWebDriver.UnproxiedWebDriver), invocation.Method))
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = unproxiedDriver;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="UnproxyingInterceptor"/>.
        /// </summary>
        /// <param name="unproxiedDriver">The unproxied WebDriver</param>
        /// <exception cref="ArgumentNullException">If <paramref name="unproxiedDriver"/> is <see langword="null" />.</exception>
        public UnproxyingInterceptor(IWebDriver unproxiedDriver)
        {
            this.unproxiedDriver = unproxiedDriver ?? throw new ArgumentNullException(nameof(unproxiedDriver));
        }
    }
}