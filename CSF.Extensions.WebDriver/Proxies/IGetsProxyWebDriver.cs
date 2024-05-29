using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using CSF.Extensions.WebDriver.Identification;
using OpenQA.Selenium;


namespace CSF.Extensions.WebDriver.Proxies
{
    public interface IGetsProxyWebDriver
    {
        IWebDriver GetProxyWebDriver(IWebDriver webDriver, ProxyCreationContext context);
    }

    public class ProxyCreationContext
    {
        public DriverOptions DriverOptions { get; set; }

        public bool AddIdentification { get; set; }
    }

    /// <summary>
    /// Factory class which creates a proxy WebDriver using Castle.DynamicProxy.
    /// </summary>
    public class WebDriverProxyFactory : IGetsProxyWebDriver
    {
        readonly IProxyGenerator proxyGenerator;
        readonly IGetsBrowserIdFromWebDriver browserIdFactory;

        /// <inheritdoc/>
        public IWebDriver GetProxyWebDriver(IWebDriver webDriver, ProxyCreationContext context)
        {
            if (webDriver is null) throw new ArgumentNullException(nameof(webDriver));
            if (context is null) throw new ArgumentNullException(nameof(context));

            var interfaces = webDriver.GetType().GetInterfaces().Except(new[] { typeof(IWebDriver), typeof(IProxyTargetAccessor) }).ToList();
            var interceptors = new List<IInterceptor>();

            interfaces.Add(typeof(IHasUnproxiedWebDriver));
            interceptors.Add(new UnproxyingInterceptor(webDriver));

            if(context.AddIdentification)
            {
                interfaces.Add(typeof(IHasBrowserId));
                interceptors.Add(new IdentificationInterceptor(browserIdFactory.GetBrowserId(webDriver, context.DriverOptions)));
            }

            var proxy = proxyGenerator.CreateInterfaceProxyWithTargetInterface(typeof(IWebDriver),
                                                                               interfaces.ToArray(),
                                                                               webDriver,
                                                                               interceptors.ToArray());
            return (IWebDriver) proxy;
        }

        public WebDriverProxyFactory(IGetsBrowserIdFromWebDriver browserIdFactory, IProxyGenerator proxyGenerator)
        {
            this.browserIdFactory = browserIdFactory ?? throw new ArgumentNullException(nameof(browserIdFactory));
            this.proxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
        }
    }

    public class UnproxyingInterceptor : IInterceptor
    {
        readonly IWebDriver unproxiedDriver;

        public void Intercept(IInvocation invocation)
        {
            if (!Is.Getter<IHasUnproxiedWebDriver>(nameof(IHasUnproxiedWebDriver.UnproxiedWebDriver), invocation.Method))
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = unproxiedDriver;
        }

        public UnproxyingInterceptor(IWebDriver unproxiedDriver)
        {
            this.unproxiedDriver = unproxiedDriver ?? throw new ArgumentNullException(nameof(unproxiedDriver));
        }
    }

    public class IdentificationInterceptor : IInterceptor
    {
        readonly BrowserId browserId;

        public void Intercept(IInvocation invocation)
        {
            if (!Is.Getter<IHasBrowserId>(nameof(IHasBrowserId.BrowserId), invocation.Method))
            {
                invocation.Proceed();
                return;
            }

            invocation.ReturnValue = browserId;
        }

        public IdentificationInterceptor(BrowserId browserId)
        {
            this.browserId = browserId ?? throw new ArgumentNullException(nameof(browserId));
        }
    }

    internal class Is
    {
        const string getterPrefix = "get_";

        internal static bool Getter<T>(string name, MethodInfo method) where T : class
            => method.DeclaringType == typeof(T) && method.Name == String.Concat(getterPrefix, name);
    }
}