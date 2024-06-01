using System;
using Semver;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// Implementation of <see cref="BrowserVersion"/> which represents a semantic version.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a version which complies with the specification at https://semver.org
    /// </para>
    /// <para>
    /// Actually, this class permits version strings which are somewhat less strict than the standards set-out at the
    /// semantic versioning website.  The <see cref="TryParse(string, out SemanticBrowserVersion, bool)"/> method makes
    /// use of the parsing functionality within <see cref="SemVersion.TryParse(string, SemVersionStyles, out SemVersion, int)"/>
    /// to permit some common improper representations of a semantic version.  The <c>TryParse</c> function in this class
    /// uses <see cref="SemVersionStyles.Any"/> to enable very generous parsing.
    /// </para>
    /// </remarks>
    public sealed class SemanticBrowserVersion : BrowserVersion
    {
        /// <summary>
        /// Gets the semantic version represented by the current instance.
        /// </summary>
        public SemVersion Version { get; }

        /// <inheritdoc/>
        public override int CompareTo(BrowserVersion other)
        {
            if (other is null || !(other is SemanticBrowserVersion semVersion)) return 1;
            return Version.CompareSortOrderTo(semVersion.Version);
        }

        /// <inheritdoc/>
        public override bool Equals(BrowserVersion other)
        {
            if (other is null || !(other is SemanticBrowserVersion semVersion)) return false;
            return Version.Equals(semVersion.Version);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Version.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => Version.ToString() + PresumedSuffix;

        /// <summary>
        /// Initialises a new instance of <see cref="SemanticBrowserVersion"/>
        /// </summary>
        /// <param name="version">The semantic version.</param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="version"/> is <see langword="null" />.</exception>
        public SemanticBrowserVersion(SemVersion version, bool isPresumed = false) : base(isPresumed)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        /// <summary>
        /// Attempts to parse the specified version string as a <see cref="SemanticBrowserVersion"/>.
        /// </summary>
        /// <param name="version">The version string</param>
        /// <param name="result">Exposes the result when this method returns <see langword="true" /></param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/></param>
        /// <returns><see langword="true" /> if the parsing succeeded; <see langword="false" /> if not.</returns>
        public static bool TryParse(string version, out SemanticBrowserVersion result, bool isPresumed = false)
        {
            if(version != null && SemVersion.TryParse(version, SemVersionStyles.Any, out var semVersion))
            {
                result = new SemanticBrowserVersion(semVersion, isPresumed);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Creates a <see cref="SemanticBrowserVersion"/> from a string, raising an exception if the string is not a valid version.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Consider using <see cref="TryParse(string, out SemanticBrowserVersion, bool)"/> if you are not certain that the version string is valid.
        /// </para>
        /// </remarks>
        /// <param name="version">The version string</param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/></param>
        /// <returns>A semantic browser version.</returns>
        /// <exception cref="FormatException">If the <paramref name="version"/> is not a valid semantic version.</exception>
        public static SemanticBrowserVersion Parse(string version, bool isPresumed = false)
        {
            return TryParse(version, out var result, isPresumed) ? result : throw new FormatException($"The version must be a valid semantic version; consider using {nameof(TryParse)} if you are not certain that the format is correct.");
        }
    }
}