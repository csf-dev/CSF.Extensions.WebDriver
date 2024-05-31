using CSF.Extensions.WebDriver.Quirks;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// An object which determines whether a specified <see cref="BrowserId"/> is a match for
    /// a specified <see cref="BrowserInfo"/>.
    /// </summary>
    public interface IGetsBrowserInfoMatch
    {
        /// <summary>
        /// Gets a value indicating whether or not the specified <paramref name="browserId"/> is a match for the
        /// specified <paramref name="browserInfo"/> or not.
        /// </summary>
        /// <param name="browserId">A browser ID</param>
        /// <param name="browserInfo">A browser info object</param>
        /// <returns><see langword="true" /> if the browser ID matches the browser info; <see langword="false" /> if not.</returns>
        /// <exception cref="ArgumentNullException">If any parameter is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <paramref name="browserInfo"/> has a <see langword="null" /> or empty <see cref="BrowserInfo.Name"/>.</exception>
        bool Matches(BrowserId browserId, BrowserInfo browserInfo);
    }
}