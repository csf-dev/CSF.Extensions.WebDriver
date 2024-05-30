using System;
using System.Collections.Generic;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// A collection of <see cref="BrowserInfo"/> indicating which browsers/version ranges are affected by a quirk.
    /// </summary>
    public class BrowserInfoCollection
    {
        ISet<BrowserInfo> affectedBrowsers = new HashSet<BrowserInfo>();

        /// <summary>
        /// Gets or sets the collection of <see cref="BrowserInfo"/> affected by the current quirk.
        /// </summary>
        public ISet<BrowserInfo> AffectedBrowsers
        {
            get => affectedBrowsers;
            set => affectedBrowsers = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}