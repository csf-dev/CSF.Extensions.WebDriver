using System;
using System.Collections.Generic;
using System.Linq;
using CSF.Extensions.WebDriver.Identification;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// Default implementation of <see cref="IGetsQuirksForBrowserId"/>.
    /// </summary>
    public class ApplicableQuirksProvider : IGetsQuirksForBrowserId
    {
        readonly IGetsBrowserInfoMatch browserMatcher;
        readonly IGetsQuirksData quirksDataProvider;

        /// <inheritdoc/>
        public IReadOnlyCollection<string> GetApplicableQuirks(BrowserId browserId)
        {
            if (browserId is null) throw new ArgumentNullException(nameof(browserId));

            return (from kvp in quirksDataProvider.GetQuirksData().Quirks
                    let quirkName = kvp.Key
                    from browserInfo in kvp.Value.AffectedBrowsers
                    where browserMatcher.Matches(browserId, browserInfo)
                    select quirkName)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Initialises a new instance of <see cref="ApplicableQuirksProvider"/>.
        /// </summary>
        /// <param name="browserMatcher">A browser matcher</param>
        /// <param name="quirksDataProvider">A provider for the source quirks data</param>
        /// <exception cref="ArgumentNullException">If <paramref name="browserMatcher"/> is <see langword="null" />.</exception>
        public ApplicableQuirksProvider(IGetsBrowserInfoMatch browserMatcher, IGetsQuirksData quirksDataProvider)
        {
            this.browserMatcher = browserMatcher ?? throw new ArgumentNullException(nameof(browserMatcher));
            this.quirksDataProvider = quirksDataProvider ?? throw new ArgumentNullException(nameof(quirksDataProvider));
        }
    }
}