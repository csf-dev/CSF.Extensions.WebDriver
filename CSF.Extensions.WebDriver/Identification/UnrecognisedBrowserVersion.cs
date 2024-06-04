using System;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// Implementation of <see cref="BrowserVersion"/> which works for any non-empty/non-null string representation of a version.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a very weak way to represent a version and should be used only as a fall-back when a web driver does not offer
    /// a better version representation, such as one which would be compatible with either <see cref="SemanticBrowserVersion"/>
    /// or <see cref="DottedNumericBrowserVersion"/>.
    /// </para>
    /// <para>
    /// Equality with another <see cref="UnrecognisedBrowserVersion"/> is performed using a culture-insensitive match of the <see cref="Version"/>
    /// property values. Sorting/comparison is also performed based upon the culture-invariant sort order of the version strings, which is unlikely
    /// to be very useful.
    /// </para>
    /// </remarks>
    public sealed class UnrecognisedBrowserVersion : BrowserVersion
    {
        /// <summary>
        /// Gets the version string with which the current instance was initialised.
        /// </summary>
        public string Version { get; }

        /// <inheritdoc/>
        public override int CompareTo(BrowserVersion other)
        {
            if (other is null || !(other is UnrecognisedBrowserVersion version)) return 1;
            return string.Compare(Version, version.Version, StringComparison.InvariantCulture);
        }

        /// <inheritdoc/>
        public override bool Equals(BrowserVersion other)
        {
            if (other is null || !(other is UnrecognisedBrowserVersion version)) return false;
            return Version.Equals(version.Version, StringComparison.InvariantCulture);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => Version + PresumedSuffix;

        /// <summary>
        /// Initialises a new instance of <see cref="UnrecognisedBrowserVersion"/>.
        /// </summary>
        /// <param name="version">The version string</param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/>.</param>
        /// <exception cref="ArgumentException">If <paramref name="version"/> is <see langword="null" /> or an empty string.</exception>
        public UnrecognisedBrowserVersion(string version, bool isPresumed = false) : base(isPresumed)
        {
            if(string.IsNullOrEmpty(version)) throw new ArgumentException("The version must not be null or an empty string.", nameof(version));
            Version = version;
        }

        /// <summary>
        /// Attempts to parse the specified version string as a <see cref="UnrecognisedBrowserVersion"/>.
        /// </summary>
        /// <param name="version">The version string</param>
        /// <param name="result">Exposes the result when this method returns <see langword="true" /></param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/></param>
        /// <returns><see langword="true" /> if the parsing succeeded; <see langword="false" /> if not.</returns>
        public static bool TryParse(string version, out UnrecognisedBrowserVersion result, bool isPresumed = false)
        {
            if(string.IsNullOrEmpty(version))
            {
                result = null;
                return false;
            }

            result = new UnrecognisedBrowserVersion(version, isPresumed);
            return true;
        }
    }
}