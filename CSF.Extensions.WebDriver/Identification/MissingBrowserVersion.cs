using System;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// Implementation of <see cref="BrowserVersion"/> which represents a null or empty string version.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this class only if no other browser version is compatible with the version.
    /// </para>
    /// </remarks>
    public sealed class MissingBrowserVersion : BrowserVersion
    {
        /// <inheritdoc/>
        public override int CompareTo(BrowserVersion other) => other is MissingBrowserVersion ? 0 : 1;

        /// <inheritdoc/>
        public override bool Equals(BrowserVersion other) => other is MissingBrowserVersion;

        /// <inheritdoc/>
        public override int GetHashCode() => 17;

        /// <inheritdoc/>
        public override string ToString() => $"[Missing version]";

        /// <summary>
        /// Initialises a new instance of <see cref="MissingBrowserVersion"/>.
        /// </summary>
        MissingBrowserVersion() : base(false) {}

        /// <summary>
        /// Gets a flyweight/singleton instance of <see cref="MissingBrowserVersion"/>.
        /// </summary>
        public static readonly MissingBrowserVersion Instance = new MissingBrowserVersion();
    }
}