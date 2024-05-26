using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    from constructor in type.GetConstructors().Where(OptionsConstructorPredicate)
                    where !constructor.GetParameters().Single().ParameterType.IsAbstract
                    select new WebDriverAndOptionsTypePair(type, constructor.GetParameters().Single().ParameterType))
                .ToArray();
        }

        /// <summary>
        /// Gets a predicate function which matches a constructor that has only one parameter and that parameter
        /// derives from <see cref="DriverOptions"/>.
        /// </summary>
        internal static Func<ConstructorInfo,bool> OptionsConstructorPredicate
        {
            get {
                return constructor => constructor.GetParameters().Length == 1
                                   && typeof(DriverOptions).IsAssignableFrom(constructor.GetParameters().Single().ParameterType);
            }
        }
   }
}