using System.Threading;
using CSF.Extensions.WebDriver.Identification;
using CSF.Extensions.WebDriver.Quirks;
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

        /// <summary>
        /// Gets or sets a value indicating whether or not the created proxy should be enriched with the functionality
        /// of <see cref="IHasQuirks" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this property is <see langword="true" /> then the value of <see cref="AddIdentification"/> is irrelevant;
        /// the functionality of <see cref="IHasBrowserId"/> will always be added to the proxy, as this quirks functionality
        /// depends upon it.
        /// </para>
        /// </remarks>
        public bool AddQuirks { get; set; }
    }
}