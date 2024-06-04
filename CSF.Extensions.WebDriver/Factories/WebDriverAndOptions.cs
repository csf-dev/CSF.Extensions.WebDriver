using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>A model containing a WebDriver and the <see cref="DriverOptions"/> which were used to create it.</summary>
    public class WebDriverAndOptions
    {
        /// <summary>Gets the WebDriver</summary>
        public IWebDriver WebDriver { get; }

        /// <summary>Gets the <see cref="DriverOptions"/> which were used to create the <see cref="WebDriver"/></summary>
        public DriverOptions DriverOptions { get; }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverAndOptions"/>.
        /// </summary>
        /// <param name="webDriver">The WebDriver.</param>
        /// <param name="driverOptions">The options which were used to create <paramref name="webDriver"/>.</param>
        /// <exception cref="ArgumentNullException">If either parameter is <see langword="null" />.</exception>
        public WebDriverAndOptions(IWebDriver webDriver, DriverOptions driverOptions)
        {
            WebDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            DriverOptions = driverOptions ?? throw new ArgumentNullException(nameof(driverOptions));
        }
    }
}