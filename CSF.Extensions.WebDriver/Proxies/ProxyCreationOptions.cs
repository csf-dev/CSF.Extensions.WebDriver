using CSF.Extensions.WebDriver.Identification;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Options related to the creation of a proxy <see cref="IWebDriver"/>.
    /// </summary>
    public class ProxyCreationOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="DriverOptions"/> with which the WebDriver was originally created.
        /// </summary>
        public DriverOptions DriverOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the created proxy should be enriched with the functionality
        /// of <see cref="IHasBrowserId" />.
        /// </summary>
        public bool AddIdentification { get; set; }
    }
}