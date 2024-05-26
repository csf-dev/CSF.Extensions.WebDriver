using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Factory service which creates implementations of <see cref="RemoteWebDriver"/> from <see cref="WebDriverCreationOptions"/>.
    /// </summary>
    public class RemoteWebDriverFromOptionsFactory : ICreatesWebDriverFromOptions
    {
        /// <inheritdoc/>
        public IWebDriver GetWebDriver(WebDriverCreationOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            
            return string.IsNullOrWhiteSpace(options.GridUrl)
                ? new RemoteWebDriver(options.Options)
                : new RemoteWebDriver(new Uri(options.GridUrl), options.Options);
        }
    }
}