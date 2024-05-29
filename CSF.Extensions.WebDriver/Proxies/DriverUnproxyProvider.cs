using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Proxies
{
    /// <summary>
    /// Simple container type which intentinoally 'leaks' the unproxied WebDriver implementation.
    /// </summary>
    public sealed class DriverUnproxyProvider : IHasUnproxiedWebDriver
    {
        /// <inheritdoc/>
        public IWebDriver UnproxiedWebDriver { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="DriverUnproxyProvider"/>.
        /// </summary>
        /// <param name="driver">The original/unproxied WebDriver.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="driver"/> is <see langword="null" />.</exception>
        public DriverUnproxyProvider(IWebDriver driver)
        {
            UnproxiedWebDriver = driver ?? throw new ArgumentNullException(nameof(driver));
        }
    }
}