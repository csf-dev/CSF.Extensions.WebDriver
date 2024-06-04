using System;
using OpenQA.Selenium;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// A model which represents a combination of browser, OS platform and the browser version.
    /// </summary>
    public sealed class BrowserId : IEquatable<BrowserId>
    {
        internal const string
            UnknownBrowser = "Unknown browser",
            UnknownPlatform = "Unknown platform";

        /// <summary>
        /// Gets the browser name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the operating system platform upon which the browser is running.
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// Gets a model which represents the browser's version.
        /// </summary>
        public BrowserVersion Version { get; }

        /// <inheritdoc/>
        public bool Equals(BrowserId other)
        {
            if (other is null) return false;
            if (ReferenceEquals(other, this)) return true;

            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(Platform, other.Platform, StringComparison.InvariantCultureIgnoreCase)
                && Equals(Version, other.Version);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as BrowserId);

        /// <inheritdoc/>
        public override int GetHashCode() => Name.GetHashCode() ^ Platform.GetHashCode() ^ Version.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => $"{Name} ({Platform}): {Version}";

        /// <summary>
        /// Initialises a new instance of <see cref="BrowserId"/>.
        /// </summary>
        /// <param name="name">The browser name</param>
        /// <param name="platform">The browser platform</param>
        /// <param name="version">The browser version</param>
        /// <exception cref="ArgumentException">If either <paramref name="name"/> or <paramref name="platform"/> is <see langword="null" /> or an empty string.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="version"/> is <see langword="null" />.</exception>
        public BrowserId(string name, string platform, BrowserVersion version)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(platform)) throw new ArgumentException($"'{nameof(platform)}' cannot be null or empty.", nameof(platform));

            Name = name;
            Platform = platform;
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        /// <summary>
        /// Gets a value indicating whether or not the browser ID equals the specified object.
        /// </summary>
        /// <param name="id">A browser ID</param>
        /// <param name="other">Another object</param>
        /// <returns><see langword="true" /> if the browser ID and the object are equal; <see langword="false" /> if not.</returns>
        public static bool operator ==(BrowserId id, object other) => ReferenceEquals(id, other) || (!(id is null) && id.Equals(other));

        /// <summary>
        /// Gets a value indicating whether or not the browser ID is not equal to the specified object.
        /// </summary>
        /// <param name="id">A browser ID</param>
        /// <param name="other">Another object</param>
        /// <returns><see langword="true" /> if the browser ID and the object are not equal; <see langword="false" /> if they are.</returns>
        public static bool operator !=(BrowserId id, object other) => !(id == other);
    }
}