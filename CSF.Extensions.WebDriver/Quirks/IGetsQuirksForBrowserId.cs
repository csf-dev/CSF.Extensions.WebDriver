using System;
using System.Collections.Generic;
using CSF.Extensions.WebDriver.Identification;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// An object which - given a <see cref="BrowserId"/> and <see cref="QuirksData"/> - can get
    /// a collection of the quirks which are applicable to that browser.
    /// </summary>
    public interface IGetsQuirksForBrowserId
    {
        /// <summary>
        /// Gets a collection of the named quirks which are applicable to the specified browser, using the
        /// specified quirks source data.
        /// </summary>
        /// <param name="browserId">The browser ID</param>
        /// <param name="quirks">The quirks source data</param>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        IReadOnlyCollection<string> GetApplicableQuirks(BrowserId browserId, QuirksData quirks);
    }
}