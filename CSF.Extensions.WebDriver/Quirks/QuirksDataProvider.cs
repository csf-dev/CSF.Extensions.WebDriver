using System;
using Microsoft.Extensions.Options;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// Implementation of <see cref="IGetsQuirksData"/> which can merge together a primary and secondary
    /// source of quirks data, providing data-shadowing capabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type provides two constructors and its behaviour will differ depending upon which is used.
    /// If this object is constructed with only one parameter, either a <see cref="QuirksData"/> or
    /// an <see cref="IOptions{TOptions}"/> of <see cref="QuirksData"/> - but not both - then the
    /// <see cref="GetQuirksData"/> method will return an object which contains the same values as that source data.
    /// </para>
    /// <para>
    /// If the constructor <see cref="QuirksDataProvider(IOptions{QuirksData},QuirksData)"/> is used with both
    /// parameters specified then the return value from <see cref="GetQuirksData"/> will be an object that contains
    /// data resulting a merge of the data from those two sources.
    /// </para>
    /// <para>
    /// The merge algorithm used in the scenario noted above is:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Where the options-based source data and the plain object data specify quirks that the
    /// other does not have, the result will have both quirks</description></item>
    /// <item><description>Where both the options-based and the plain object source data specify the same quirk,
    /// the affected browsers from the options-based data will shadow (aka 'replace' or 'override') those from the plain
    /// object</description></item>
    /// <item><description>Neither input object will be altered by this process; the results from
    /// <see cref="GetQuirksData"/> will be based upon a new object</description></item>
    /// </list>
    /// <para>
    /// The purpose of this algorithm is to permit an application to ship with a set of hard-coded quirks data.  This
    /// might be an in-memory object or read from a JSON embedded resource or similar. This in-memory data may then
    /// be added-to and/or overridden (shadowed) by end-user-specified options, using the Microsoft Options Pattern.
    /// This allows end consumers to quickly react to changes in browser support via an options/configuration change,
    /// even if a library which provides a set of quirks data has not yet been updated.
    /// </para>
    /// </remarks>
    public class QuirksDataProvider : IGetsQuirksData
    {
        readonly QuirksData data;

        /// <inheritdoc/>
        public QuirksData GetQuirksData() => data;

        QuirksData MergeQuirksData(QuirksData primary, QuirksData secondary = null)
        {
            if (primary is null) throw new ArgumentNullException(nameof(primary));
            if (secondary is null) return primary.DeepCopy();

            var result = secondary.DeepCopy();
            foreach (var quirkName in primary.Quirks.Keys)
                result.Quirks[quirkName] = primary.Quirks[quirkName].DeepCopy();

            return result;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="QuirksDataProvider"/> with only some in-memory source data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor will initialise the object using a direct copy/clone of the data within <paramref name="quirksData"/>.
        /// See the remarks upon <see cref="QuirksDataProvider"/> for more information about the available constructors.
        /// </para>
        /// </remarks>
        /// <param name="quirksData">The source quirks data</param>
        /// <exception cref="ArgumentNullException">If <paramref name="quirksData"/> parameter is <see langword="null" />.</exception>
        /// <seealso cref="QuirksDataProvider"/>
        public QuirksDataProvider(QuirksData quirksData)
        {
            if (quirksData is null) throw new ArgumentNullException(nameof(quirksData));
            
            data = MergeQuirksData(quirksData);
        }
        
        /// <summary>
        /// Initialises a new instance of <see cref="QuirksDataProvider"/> with quirks data options provided by the
        /// Microsoft Options Pattern, and optionally some in-memory source data with which to merge the options data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <paramref name="quirksData"/> is <see langword="null" /> or empty then this constructor will
        /// initialise the object using a direct copy/clone of the data within <paramref name="optionsData"/>.
        /// If both parameters are specified then the merge algorithm described in the remarks of
        /// <see cref="QuirksDataProvider"/> will be used to merge these two data-sources together.
        /// </para>
        /// </remarks>
        /// <param name="optionsData">Source quirks data from the Microsoft Options Pattern</param>
        /// <param name="quirksData">In-memory source quirks data</param>
        /// <exception cref="ArgumentNullException">If <paramref name="optionsData"/> parameter is <see langword="null" />.</exception>
        /// <seealso cref="QuirksDataProvider"/>
        public QuirksDataProvider(IOptions<QuirksData> optionsData, QuirksData quirksData = null)
        {
            if (optionsData is null) throw new ArgumentNullException(nameof(optionsData));

            data = MergeQuirksData(optionsData.Value, quirksData ?? QuirksData.Empty);
        }
    }
}