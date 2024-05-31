using System;
using System.Collections.Generic;
using CSF.Extensions.WebDriver.Identification;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// An object which - given a <see cref="BrowserId"/> - can get
    /// a collection of the quirks which are applicable to that browser.
    /// </summary>
    public interface IGetsQuirksForBrowserId
    {
        /// <summary>
        /// Gets a collection of the named quirks which are applicable to the specified browser.
        /// </summary>
        /// <param name="browserId">The browser ID</param>
        /// <exception cref="ArgumentNullException">If <paramref name="browserId"/> is <see langword="null" />.</exception>
        IReadOnlyCollection<string> GetApplicableQuirks(BrowserId browserId);
    }
}