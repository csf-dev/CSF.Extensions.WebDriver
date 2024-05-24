using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Service which scans the Selenium WebDriver assembly for web driver types.
    /// </summary>
    public class SeleniumDriverAndOptionsScanner : IGetsWebDriverWithDeterministicOptionsTypes
    {
        /// <inheritdoc/>
        public IReadOnlyCollection<WebDriverAndOptionsTypePair> GetWebDriverAndDeterministicOptionsTypes()
        {
            return (from type in typeof(IWebDriver).Assembly.GetExportedTypes()
                    where
                        type.IsClass
                        && typeof(IWebDriver).IsAssignableFrom(type)
                        && !type.IsAbstract
                        && type != typeof(OpenQA.Selenium.Remote.RemoteWebDriver)
                    from constructor in type.GetConstructors()
                    let ctorParams = constructor.GetParameters()
                    where ctorParams.Length == 1
                    let optionsParam = ctorParams.Single()
                    where
                        typeof(DriverOptions).IsAssignableFrom(optionsParam.ParameterType)
                        && !optionsParam.ParameterType.IsAbstract
                    select new WebDriverAndOptionsTypePair(type, optionsParam.ParameterType))
                .ToArray();
        }
   }
}