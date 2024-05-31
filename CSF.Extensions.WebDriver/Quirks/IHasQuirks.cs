using System.Collections.Generic;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// An object which can indicate whether or not it is affected by specified browser quirks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are many implementations of <see cref="IWebDriver"/>, each representing a different web browser.
    /// Due to design choices in these web browsers, differences in OS platform and perhaps even bugs in some versions of
    /// the browser or WebDriver implementations, browsers might exhibit quirky behaviour which differs from the uniform
    /// results which a perfect implementation would be expected to provide.
    /// </para>
    /// <para>
    /// This 'quirks' mechanism provides a way in which consumers of WebDrivers may gracefully deal with that, with
    /// minimal compromise to their design.  A quirk is a named boolean flag, such as (a fictitious quirk) <c>CannotDisplayYellow</c>.
    /// Consumers of the WebDriver may use this interface to determine whether or not the WebDriver is affected by this quirk
    /// or not, by using the <see cref="HasQuirk(string)"/> function.
    /// If the WebDriver is known to be affected by the quirk then that method will return <see langword="true" />, or if it is
    /// not affected then the method will return <see langword="false" />.
    /// The consumer may then take whatever course of action is required in order to work-around that quirk, if the result was
    /// <see langword="true" />.
    /// </para>
    /// <para>
    /// The implementation of this interface uses the <see cref="Identification.BrowserId"/> present on the WebDriver to identify it.
    /// It then cross-references this with source data which lists which browser, platform and version ranges are affected by which
    /// quirks.
    /// It is intended that the source data listing the quirks and which browsers, platforms &amp; browser version-ranges are affected
    /// come from configuration data.
    /// This way new quirks may be added without requiring code-changes. Quirks which are no longer relevant (for example, a bug
    /// is patched) may also be retired via configuration.
    /// </para>
    /// <para>
    /// The actual names of the quirks may be completely arbitrary strings.  Where possible it should be a semi-human-readable identification
    /// of a piece of functionality which does not work in the expected/common manner.  IE:  It should indicate something that a
    /// particular WebDriver or browser implementation cannot do, or that requires a specific workaround in order to achieve the same
    /// results as other WebDriver/browser implementations.
    /// </para>
    /// </remarks>
    public interface IHasQuirks
    {
        /// <summary>
        /// Gets a collection of the names of all of the quirks which affect the current object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Prefer using <see cref="HasQuirk(string)"/> instead of this property.
        /// </para>
        /// </remarks>
        IReadOnlyCollection<string> AllQuirks { get; }

        /// <summary>
        /// Gets a value indicating whether or not the current object is affected by the specified named quirk.
        /// </summary>
        /// <para>
        /// For more information on what quirks are, see the documentation for <see cref="IHasQuirks"/>.
        /// </para>
        /// <param name="quirkName">The name of a quirk</param>
        /// <returns><see langword="true" /> if the object is affected by the specified quirk; <see langword="false" /> otherwise.</returns>
        bool HasQuirk(string quirkName);
    }
}