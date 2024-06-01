using System.Collections.Generic;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// An object which can get the WebDriver implementation types which have deterministic options types.
    /// </summary>
    public interface IGetsWebDriverWithDeterministicOptionsTypes
    {
        /// <summary>
        /// Gets a collection of the implementations of <see cref="IWebDriver"/> in the Selenium library which
        /// have deterministic options types.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will only get concrete implementations of <see cref="IWebDriver"/> for which we can reliably determine the
        /// associated/corresponding concrete implementation of <see cref="DriverOptions"/>.  For example, this method will not return
        /// <see cref="OpenQA.Selenium.Remote.RemoteWebDriver"/> because that takes one of many different options types.
        /// On the other hand, as of the time of writing, <see cref="OpenQA.Selenium.Chrome.ChromeDriver"/> would be included in the results, because it has
        /// a constructor which uniquely and unambiguously identifies that it requires an instance of <see cref="OpenQA.Selenium.Chrome.ChromeOptions"/>
        /// </para>
        /// <para>
        /// This method makes use of assembly-scanning techniques to get the list of drivers and options, so it stands at least some chance of correctly
        /// dealing with alterations in the Selenium library, such as the introduction of new WebDrivers.
        /// </para>
        /// </remarks>
        /// <returns>A collection of  web driver types and their associated options types.</returns>
        IReadOnlyCollection<WebDriverAndOptionsTypePair> GetWebDriverAndDeterministicOptionsTypes();
    }
}