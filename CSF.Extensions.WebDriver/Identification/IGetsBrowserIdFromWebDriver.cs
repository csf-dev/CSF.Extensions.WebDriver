using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// An object which can get browser identification from a WebDriver.
    /// </summary>
    public interface IGetsBrowserIdFromWebDriver
    {
        /// <summary>
        /// Gets a <see cref="BrowserId"/> from the specified <see cref="IWebDriver"/> and the options
        /// object with which it was created.
        /// </summary>
        /// <param name="driver">The WebDriver</param>
        /// <param name="creationOptions">The driver options with which the driver was created</param>
        /// <returns>A <see cref="BrowserId"/></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="driver"/> is <see langword="null" />.</exception>
        BrowserId GetBrowserId(IWebDriver driver, DriverOptions creationOptions);
    }
}