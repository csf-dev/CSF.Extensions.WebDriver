using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// Provide type for <see cref="IWebDriver"/> and <see cref="DriverOptions"/> types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this type perform internal caching, to avoid repeatedly re-using the computationally-expensive
    /// functionality of <see cref="IGetsWebDriverWithDeterministicOptionsTypes"/>.  This should be safe under almost
    /// all circumstances, unless dynamic assembly unload/reload occurs.
    /// </para>
    /// </remarks>
    public class WebDriverTypesProvider : IGetsWebDriverAndOptionsTypes
    {
        readonly IGetsWebDriverWithDeterministicOptionsTypes deterministicWebDriverTypesScanner;

        readonly object
            webDriverAndDeterministicOptionsTypesSyncRoot = new object(),
            supportedShorthandDriverTypesSyncRoot = new object();

        /// <summary>
        /// A cache of the implementations of <see cref="IWebDriver"/> which are shipped with Selenium, and the
        /// implementation types of <see cref="DriverOptions"/> which they use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The keys to this dictionary are the web driver types.
        /// This field should only ever be set by <see cref="GetWebDriverAndDeterministicOptionsTypes"/>, which ensures thread-safety.
        /// </para>
        /// </remarks>
        IReadOnlyDictionary<Type, WebDriverAndOptionsTypePair> webDriverAndDeterministicOptionsTypes;

        /// <summary>
        /// A cache of the implementation types of <see cref="IWebDriver"/> for which this library supports the use of shorthand names.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Shorthand names refers to simply the unqualified type name, like <c>ChromeDriver</c>, instead of an assembly-qualified type name.
        /// This field should only ever be set by <see cref="GetSupportedShorthandWebDriverTypes"/>, which ensures thread-safety.
        /// </para>
        /// </remarks>
        IReadOnlyCollection<Type> supportedShorthandDriverTypes;

        /// <inheritdoc/>
        public Type GetWebDriverOptionsType(Type driverType, string typeName = null)
        {
            if (driverType is null) throw new ArgumentNullException(nameof(driverType));

            var deterministicOptionsTypes = GetWebDriverAndDeterministicOptionsTypes();

            if (typeName != null)
            {
                var matchedShorthandOptionsType = deterministicOptionsTypes.Values
                    .Select(x => x.OptionsType)
                    .FirstOrDefault(x => Equals(x.Name, typeName));
                if (matchedShorthandOptionsType != null) return matchedShorthandOptionsType;

                return LoadType<DriverOptions>(typeName);
            }

            return deterministicOptionsTypes.TryGetValue(driverType, out var typePair)
                ? typePair.OptionsType
                : throw new ArgumentException($"No type name was specified but the {nameof(IWebDriver)} implementation type {driverType.FullName} does not imply an options type. For driver types which are not shipped with Selenium, or for {nameof(RemoteWebDriver)}, the options type must be specified.", nameof(typeName));
        }

        /// <inheritdoc/>
        public Type GetWebDriverType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentException($"'{nameof(typeName)}' cannot be null or whitespace.", nameof(typeName));

            return GetSupportedShorthandWebDriverTypes().FirstOrDefault(x => x.Name == typeName) ?? LoadType<IWebDriver>(typeName);
        }

        static Type LoadType<TBase>(string typeName) where TBase : class
        {
            Type result;

            try
            {
                result = Type.GetType(typeName, true);
            }
            catch(Exception e)
            {
                throw new TypeLoadException($"The specified type '{typeName}' could not be loaded.", e);
            }

            return typeof(TBase).IsAssignableFrom(result)
                ? result
                : throw new ArgumentException($"The specified type '{result.FullName}' must derive from {typeof(TBase).Name}", nameof(typeName));
        }

        /// <summary>
        /// Uses an instance of <see cref="SeleniumDriverAndOptionsScanner"/> to get the implementations of
        /// <see cref="IWebDriver"/> which have deterministic options types.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method makes use of caching in order to avoid repeating the assembly-scanning technique.  That should be
        /// safe to do, because unless we dynamically unload the Selenium assembly and then reload a different one, the
        /// results of this assembly-scanning cannot change during the application lifetime.
        /// </para>
        /// <para>
        /// Thread-safety of the cache is made certain by locking upon <see cref="webDriverAndDeterministicOptionsTypesSyncRoot"/>.
        /// </para>
        /// </remarks>
        /// <returns>A collection which lists all of the implementations of <see cref="IWebDriver"/> which have deterministic options types.</returns>
        /// <seealso cref="SeleniumDriverAndOptionsScanner"/>
        IReadOnlyDictionary<Type, WebDriverAndOptionsTypePair> GetWebDriverAndDeterministicOptionsTypes()
        {
            lock(webDriverAndDeterministicOptionsTypesSyncRoot)
            {
                return webDriverAndDeterministicOptionsTypes = webDriverAndDeterministicOptionsTypes ?? deterministicWebDriverTypesScanner
                    .GetWebDriverAndDeterministicOptionsTypes()
                    .ToDictionary(k => k.WebDriverType, v => v);
            }
        }

        /// <summary>
        /// Gets a collection of the implementation types of <see cref="IWebDriver"/> for which we support shorthand names.
        /// </summary>
        /// <returns>A collection of types.</returns>
        IReadOnlyCollection<Type> GetSupportedShorthandWebDriverTypes()
        {
            lock(supportedShorthandDriverTypesSyncRoot)
            {
                return supportedShorthandDriverTypes = supportedShorthandDriverTypes ?? GetWebDriverAndDeterministicOptionsTypes()
                    .Keys
                    .Union(new[] { typeof(RemoteWebDriver) })
                    .ToArray();
            }
        }

        /// <summary>
        /// Initialises a new instance of <see cref="WebDriverTypesProvider"/>.
        /// </summary>
        /// <param name="deterministicWebDriverTypesScanner">A scanner for the WebDriver types which imply a deterministic options type.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="deterministicWebDriverTypesScanner"/> is <see langword="null" />.</exception>
        public WebDriverTypesProvider(IGetsWebDriverWithDeterministicOptionsTypes deterministicWebDriverTypesScanner)
        {
            this.deterministicWebDriverTypesScanner = deterministicWebDriverTypesScanner ?? throw new ArgumentNullException(nameof(deterministicWebDriverTypesScanner));
        }
    }
}

