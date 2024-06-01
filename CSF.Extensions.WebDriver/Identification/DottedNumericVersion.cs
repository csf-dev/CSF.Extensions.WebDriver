using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// A <see cref="BrowserVersion"/> which is derived from a series of numeric components separated by period characters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is similar to <see cref="SemanticBrowserVersion"/> except that it is far more forgiving of unusually-formatted versions.
    /// </para>
    /// <list type="bullet">
    /// <item><description>It permits any amount of leading and trailing non-numeric characters</description></item>
    /// <item><description>It permits any number of 'version' components, not just a maximum of 3 as is the case with SemVer</description></item>
    /// </list>
    /// </remarks>
    public sealed class DottedNumericBrowserVersion : BrowserVersion
    {
        const string parserPattern = @"(\d+)(?:\.(\d+))*";
        static readonly Regex parser = new Regex(parserPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        readonly IReadOnlyList<int> components;

        /// <summary>
        /// Gets the collection of numeric version components.
        /// </summary>
        /// <value>The version components.</value>
        public IReadOnlyList<int> VersionComponents => components;

        /// <inheritdoc/>
        public override int CompareTo(BrowserVersion other)
        {
            if (other is null || !(other is DottedNumericBrowserVersion version)) return 1;

            var theirCount = version.VersionComponents.Count;
            for (var i = 0; i < VersionComponents.Count; i++)
            {
                var myComponent = VersionComponents[i];
                var theirComponent = i < theirCount ? version.VersionComponents[i] : (int?)null;

                // If the other doesn't have a component at the current position then we are 'less than' them
                if (theirComponent is null) return -1;
                if (Equals(myComponent, theirComponent.Value)) continue;
                return myComponent.CompareTo(theirComponent.Value);
            }

            // If we reach this point then we ran out of version components, if the other version
            // has more components than this one then we are 'greater than' them
            return theirCount > VersionComponents.Count ? 1 : 0;
        }

        /// <inheritdoc/>
        public override bool Equals(BrowserVersion other)
        {
            if (other is null || !(other is DottedNumericBrowserVersion version)) return false;
            return VersionComponents.SequenceEqual(version.VersionComponents);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => VersionComponents.Aggregate(17, HashFunction);

        static int HashFunction(int acc, int next) { unchecked { return acc * 23 + next; } }

        /// <inheritdoc/>
        public override string ToString() => string.Join(".", VersionComponents.Select(x => x.ToString())) + PresumedSuffix;

        /// <summary>
        /// Initialises a new instance of <see cref="DottedNumericBrowserVersion"/>
        /// </summary>
        /// <param name="versionComponents">The version components.</param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="versionComponents"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <paramref name="versionComponents"/> has a count of zero.</exception>
        public DottedNumericBrowserVersion(IList<int> versionComponents, bool isPresumed = false) : base(isPresumed)
        {
            components = versionComponents?.ToArray() ?? throw new ArgumentNullException(nameof(versionComponents));
            if (versionComponents.Count == 0) throw new ArgumentException("The version must have at least one numeric version component", nameof(versionComponents));
        }

        /// <summary>
        /// Attempts to parse the specified version string as a <see cref="DottedNumericBrowserVersion"/>.
        /// </summary>
        /// <param name="version">The version string</param>
        /// <param name="result">Exposes the result when this method returns <see langword="true" /></param>
        /// <param name="isPresumed">Whether or not this is a presumed version; see <see cref="BrowserVersion.IsPresumedVersion"/></param>
        /// <returns><see langword="true" /> if the parsing succeeded; <see langword="false" /> if not.</returns>
        public static bool TryParse(string version, out DottedNumericBrowserVersion result, bool isPresumed = false)
        {
            result = null;
            if(version is null) return false;

            var match = parser.Match(version);
            if(!match.Success) return false;

            var firstComponent = int.Parse(match.Groups[1].Value);
            var components = match.Groups[2].Captures
                .Cast<Capture>()
                .Select(x => int.Parse(x.Value))
                .ToList();
      
            components.Insert(0, firstComponent);
            result = new DottedNumericBrowserVersion(components, isPresumed);
            return true;
        }

    }
}
