namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// Information which identifies a web browser and range of versions which are affected by a quirk.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The only truly mandatory property in this model is <see cref="Name"/>.
    /// It is not expected to be likely that both of <see cref="MinVersion"/> &amp; <see cref="MaxVersion"/>
    /// are omitted though.  Such a situation would mean "every version in existence".
    /// </para>
    /// </remarks>
    public class BrowserInfo
    {
        /// <summary>
        /// Gets or sets the browser name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is mandatory, identifying a browser name which this quirk affects.
        /// </para>
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the browser OS platform.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is optional, if omitted then it means that the quirk does not depend upon the OS platform.
        /// If specified then the quirk will only be interpreted as affecting the browser on the matching OS platform.
        /// </para>
        /// </remarks>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the minimum version (inclusive) which is affected by the quirk.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is optional, if omitted then it means "All versions from the very first version up to the <see cref="MaxVersion"/>".
        /// </para>
        /// </remarks>
        public string MinVersion { get; set; }

        /// <summary>
        /// Gets or sets the maximum version (inclusive) which is affected by the quirk.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is optional, if omitted then it means "All versions from the <see cref="MinVersion"/> &amp; up" with
        /// no maximum.
        /// </para>
        /// </remarks>
        public string MaxVersion { get; set; }

        internal BrowserInfo DeepCopy() => new BrowserInfo { Name = Name, Platform = Platform, MinVersion = MinVersion, MaxVersion = MaxVersion };
    }
}