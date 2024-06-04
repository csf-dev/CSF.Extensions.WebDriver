namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// An object which can augment a <see cref="WebDriverProxyCreationContext"/> which will subsequently be used to create a
    /// proxy <see cref="OpenQA.Selenium.IWebDriver"/>.
    /// </summary>
    public interface IAugmentsProxyContext
    {
        /// <summary>
        /// Augment the specified <see cref="WebDriverProxyCreationContext"/> with additional functionality.
        /// </summary>
        /// <param name="context">The context object to augment.</param>
        void AugmentContext(WebDriverProxyCreationContext context);
    }
}