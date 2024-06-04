namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// An object which can get the source data for browser quirks.
    /// </summary>
    public interface IGetsQuirksData
    {
        /// <summary>
        /// Gets the source data for browser quirks, using state available in the current instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Typically this data would come from dependency injected services/options.  See
        /// <see cref="ServiceCollectionExtensions.AddWebDriverQuirks(Microsoft.Extensions.DependencyInjection.IServiceCollection, QuirksData, bool, string)"/>
        /// for more information about configuring this in a typical app.
        /// </para>
        /// </remarks>
        QuirksData GetQuirksData();
    }
}