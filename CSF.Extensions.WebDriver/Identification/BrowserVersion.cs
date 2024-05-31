using System;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// Base class for a model which describes a WebDriver browser version.
    /// </summary>
    public abstract class BrowserVersion : IEquatable<BrowserVersion>, IComparable<BrowserVersion>
    {
        /// <summary>
        /// Gets a value indicating whether or not the current instance represents a presumed version or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this property is <see langword="true" />, this indicates that we are not sure that the version reported by
        /// the current <see cref="BrowserVersion"/> is accurate. This can be the case when the WebDriver implementation does
        /// not report-back its version correctly.
        /// </para>
        /// <para>
        /// A presumed version is created by using the information from the original WebDriver request (based upon the options used
        /// to create the driver).  It means "We asked for version X, so we presume that it is version X".  Of course, across the
        /// many scenarios possible, this might not always be correct.
        /// </para>
        /// </remarks>
        public bool IsPresumedVersion { get; }

        /// <summary>
        /// Gets a suffix for derived types' versions which indicates whether or not <see cref="IsPresumedVersion"/>
        /// is <see langword="true" /> or not.
        /// </summary>
        protected string PresumedSuffix => IsPresumedVersion ? " (presumed)" : string.Empty;

        /// <inheritdoc/>
        public abstract int CompareTo(BrowserVersion other);

        /// <inheritdoc/>
        public abstract bool Equals(BrowserVersion other);

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as BrowserVersion);

        /// <inheritdoc/>
        public abstract override int GetHashCode();

        /// <summary>
        /// Initialises a new instance of <see cref="BrowserVersion"/>.
        /// </summary>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="IsPresumedVersion"/>.</param>
        protected internal BrowserVersion(bool isPresumed = false)
        {
            IsPresumedVersion = isPresumed;
        }

        /// <summary>
        /// Gets a value indicating whether or not the browser version is equal to the specified object.
        /// </summary>
        /// <param name="version">A browser version</param>
        /// <param name="other">Another object</param>
        /// <returns><see langword="true" /> if the version is equal to the object; <see langword="false" /> if not.</returns>
        public static bool operator ==(BrowserVersion version, object other) => ReferenceEquals(version, other) || (!(version is null) && version.Equals(other));

        /// <summary>
        /// Gets a value indicating whether or not the browser version is not equal to the specified object.
        /// </summary>
        /// <param name="version">A browser version</param>
        /// <param name="other">Another object</param>
        /// <returns><see langword="true" /> if the version is not equal to the object; <see langword="false" /> if it is.</returns>
        public static bool operator !=(BrowserVersion version, object other) => !(version == other);

        /// <summary>
        /// Gets a value indicating whether or not one browser version is greater than the other.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If either parameter is <see langword="null" /> then this operator will return <see langword="false" />.
        /// </para>
        /// </remarks>
        /// <param name="first">A browser version</param>
        /// <param name="second">A browser version</param>
        /// <returns><see langword="true" /> if the <paramref name="first"/> version is greater than the <paramref name="second"/>; <see langword="false" /> otherwise.</returns>
        public static bool operator >(BrowserVersion first, BrowserVersion second)
        {
            if (first is null || second is null) return false;
            return first.CompareTo(second) > 0;
        }

        /// <summary>
        /// Gets a value indicating whether or not one browser version is less than the other.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If either parameter is <see langword="null" /> then this operator will return <see langword="false" />.
        /// </para>
        /// </remarks>
        /// <param name="first">A browser version</param>
        /// <param name="second">A browser version</param>
        /// <returns><see langword="true" /> if the <paramref name="first"/> version is less than the <paramref name="second"/>; <see langword="false" /> otherwise.</returns>
        public static bool operator <(BrowserVersion first, BrowserVersion second)
        {
            if (first is null || second is null) return false;
            return first.CompareTo(second) < 0;
        }

        /// <summary>
        /// Gets a value indicating whether or not one browser version is greater than or equal to the other.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If either parameter is <see langword="null" /> then this operator will return <see langword="false" />.
        /// </para>
        /// </remarks>
        /// <param name="first">A browser version</param>
        /// <param name="second">A browser version</param>
        /// <returns><see langword="true" /> if the <paramref name="first"/> version is greater than or equal to the <paramref name="second"/>; <see langword="false" /> otherwise.</returns>
        public static bool operator >=(BrowserVersion first, BrowserVersion second)
        {
            if (first is null || second is null) return false;
            return first.CompareTo(second) >= 0;
        }

        /// <summary>
        /// Gets a value indicating whether or not one browser version is less than or equal to the other.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If either parameter is <see langword="null" /> then this operator will return <see langword="false" />.
        /// </para>
        /// </remarks>
        /// <param name="first">A browser version</param>
        /// <param name="second">A browser version</param>
        /// <returns><see langword="true" /> if the <paramref name="first"/> version is less than or equal to the <paramref name="second"/>; <see langword="false" /> otherwise.</returns>
        public static bool operator <=(BrowserVersion first, BrowserVersion second)
        {
            if (first is null || second is null) return false;
            return first.CompareTo(second) <= 0;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BrowserVersion"/> from the specified version received from a WebDriver and the version which was
        /// requested via the driver options.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method attempts to find the 'best' implementation of <see cref="BrowserVersion"/> available.
        /// It will try <see cref="SemanticBrowserVersion"/> and then <see cref="DottedNumericBrowserVersion"/> for both the
        /// <paramref name="version"/> and then <paramref name="requestedVersion"/> before it attempts <see cref="UnrecognisedBrowserVersion"/>
        /// with either.
        /// </para>
        /// </remarks>
        /// <param name="version">A browser version string received from a WebDriver</param>
        /// <param name="requestedVersion">An optional browser version string which was 'requested' by inclusion in the WebDriver's creation options.</param>
        /// <returns>An implementation of <see cref="BrowserVersion"/></returns>
        public static BrowserVersion Create(string version, string requestedVersion = null)
        {
            if (SemanticBrowserVersion.TryParse(version, out var semVersion)) return semVersion;
            if (DottedNumericBrowserVersion.TryParse(version, out var numericVersion)) return numericVersion;
            if (SemanticBrowserVersion.TryParse(requestedVersion, out var requestedSemVersion, true)) return requestedSemVersion;
            if (DottedNumericBrowserVersion.TryParse(requestedVersion, out var requestedNumericVersion)) return requestedNumericVersion;
            if (UnrecognisedBrowserVersion.TryParse(version, out var unrecognisedVersion)) return unrecognisedVersion;
            if (UnrecognisedBrowserVersion.TryParse(requestedVersion, out var requestedUnrecognisedVersion)) return requestedUnrecognisedVersion;

            return MissingBrowserVersion.Instance;
        }
    }
}