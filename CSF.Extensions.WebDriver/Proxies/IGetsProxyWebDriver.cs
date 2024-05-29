using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    public interface IGetsProxyWebDriver
    {
        IWebDriver GetProxyWebDriver(IWebDriver webDriver, ProxyCreationContext context);
    }

    public class ProxyCreationContext
    {
        public bool AddIdentification { get; set; }
    }

    public class WebDriverProxyFactory : IGetsProxyWebDriver
    {
        /// <inheritdoc/>
        public IWebDriver GetProxyWebDriver(IWebDriver webDriver, ProxyCreationContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}