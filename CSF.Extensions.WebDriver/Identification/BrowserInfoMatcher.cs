using System;
using CSF.Extensions.WebDriver.Quirks;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// Default implementation of <see cref="IGetsBrowserInfoMatch"/>.
    /// </summary>
    public class BrowserInfoMatcher : IGetsBrowserInfoMatch
    {
        /// <inheritdoc/>
        public bool Matches(BrowserId browserId, BrowserInfo browserInfo)
        {
            if (browserId is null) throw new ArgumentNullException(nameof(browserId));
            if (browserInfo is null) throw new ArgumentNullException(nameof(browserInfo));
            if (string.IsNullOrEmpty(browserInfo.Name)) throw new ArgumentException("The browser name must not be null or an empty string", nameof(browserInfo));

            if (!string.Equals(browserId.Name, browserInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(browserInfo.Platform)
             && !string.Equals(browserId.Platform, browserInfo.Platform, StringComparison.InvariantCultureIgnoreCase))
                return false;

            var minVersion = GetVersionOrNull(browserInfo.MinVersion);
            var maxVersion = GetVersionOrNull(browserInfo.MaxVersion);

            return (minVersion == null || browserId.Version <= minVersion)
                && (maxVersion == null || browserId.Version >= maxVersion);
        }

        static BrowserVersion GetVersionOrNull(string version)
            => !string.IsNullOrEmpty(version) ? BrowserVersion.Create(version) : null;
    }
}