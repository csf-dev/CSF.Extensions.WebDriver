using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Factories
{
    /// <summary>
    /// An object which gets the <see cref="IWebDriver"/> and <see cref="DriverOptions"/> types.
    /// </summary>
    public interface IGetsWebDriverAndOptionsTypes
    {
        /// <summary>
        /// Gets the Type for an implementation of <see cref="IWebDriver"/>, from a string type name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For WebDriver implementations which are shipped with Selenium, all that is required is the
        /// short name, corresponding to <see cref="Type.Name"/>.
        /// For WebDriver implementations that are not part of the <c>Selenium.WebDriver</c> NuGet package,
        /// this should be an assembly-qualified type name, such that the type could be located with
        /// <see cref="Type.GetType(string)"/>.
        /// </para>
        /// </remarks>
        /// <param name="typeName">The name of the type.</param>
        /// <returns>A WebDriver type.</returns>
        /// <exception cref="ArgumentException">If <paramref name="driverType"/> is <see langword="null" /> or whitespace,
        /// or if it corresponds to a type which does not implement <see cref="IWebDriver"/>.</exception>
        /// <exception cref="TypeLoadException">If the type specified by <paramref name="typeName"/> cannot be loaded.</exception>
        Type GetWebDriverType(string typeName);

        /// <summary>
        /// Gets the Type for an implementation of <see cref="DriverOptions"/> from an optional string type name and
        /// the implementation-type of the <see cref="IWebDriver"/> which will be consuming those options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Under some circumstances, the <paramref name="typeName"/> parameter may be <see langword="null" /> and the type will
        /// still be correctly determined.  That is because a number of <see cref="IWebDriver"/> implementation types imply a
        /// deterministic <see cref="DriverOptions"/> type via their constructor signature.
        /// </para>
        /// <para>
        /// For such WebDriver implementation types, if the type name is omitted then the options type will be returned based upon
        /// the implied options type, which the driver type takes in its constructor.
        /// </para>
        /// <para>
        /// Additionally, for all concrete <see cref="DriverOptions"/> implementations which are shipped with Selenium, when
        /// <paramref name="typeName"/> is specified it needs only be a short type name, equivalent to <see cref="Type.Name"/>.
        /// For third-party driver options implementations, this must be an assembly-qualified name, such that the type may
        /// be found using <see cref="Type.GetType(string)"/>.
        /// </para>
        /// </remarks>
        /// <param name="driverType">The type of <see cref="IWebDriver"/> which will be consuming these options.</param>
        /// <param name="typeName">An optional name of a type which implements <see cref="DriverOptions"/>; see the remarks for more information.</param>
        /// <returns>A DriverOptions type.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="driverType"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <paramref name="typeName"/> is specified, but does not correspond to a type which derives
        /// from <see cref="DriverOptions"/>, or if <paramref name="typeName"/> is <see langword="null" /> but the <paramref name="driverType"/> is not
        /// a type which implies a driver options type.</exception>
        /// <exception cref="TypeLoadException">If the type specified by <paramref name="typeName"/> cannot be loaded.</exception>
        Type GetWebDriverOptionsType(Type driverType, string typeName = null);

        /// <summary>
        /// Gets the Type for an implementation of <see cref="ICreatesWebDriverFromOptions"/>, for when a third-party factory is to be used.
        /// </summary>
        /// <param name="typeName">The assembly qualified name of the factory type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If <paramref name="typeName"/> is <see langword="null" /> or whitespace,
        /// or if it corresponds to a type which does not implement <see cref="ICreatesWebDriverFromOptions"/>.</exception>
        /// <exception cref="TypeLoadException">If the type specified by <paramref name="typeName"/> cannot be loaded.</exception>
        Type GetWebDriverFactoryType(string typeName);
    }
}