using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// Default implementation of <see cref="IGetsBrowserIdFromWebDriver"/>.
    /// </summary>
    public class BrowserIdFactory : IGetsBrowserIdFromWebDriver
    {
        internal const string
            BrowserNameCapability = "browserName",
            PlatformNameCapability = "platformName",
            BrowserVersionCapability = "browserVersion";

        /// <inheritdoc/>
        public BrowserId GetBrowserId(IWebDriver driver, DriverOptions creationOptions)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));

            var caps = driver as IHasCapabilities;

            var actualName = (string) caps?.Capabilities.GetCapability(BrowserNameCapability);
            var creationName = creationOptions?.BrowserName;
            var actualPlatform = (string) caps?.Capabilities.GetCapability(PlatformNameCapability);
            var creationPlatform = creationOptions?.PlatformName;
            var actualVersion = (string)caps?.Capabilities.GetCapability(BrowserVersionCapability);
            var creationVersion = creationOptions?.BrowserVersion;

            var name = !string.IsNullOrEmpty(actualName)
                ? actualName
                : !string.IsNullOrEmpty(creationName)
                ? creationName
                : BrowserId.UnknownBrowser;
            var platform = !string.IsNullOrEmpty(actualPlatform)
                ? actualPlatform
                : !string.IsNullOrEmpty(creationPlatform)
                ? creationPlatform
                : BrowserId.UnknownPlatform;
            var version = BrowserVersion.Create(actualVersion, creationVersion);

            return new BrowserId(name, platform, version);
        }
    }
}