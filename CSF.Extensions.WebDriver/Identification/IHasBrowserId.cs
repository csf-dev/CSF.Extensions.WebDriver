namespace CSF.Extensions.WebDriver.Identification
{
    /// <summary>
    /// An object which provides browser identification information.
    /// </summary>
    public interface IHasBrowserId
    {
        /// <summary>
        /// Gets identification information for the web browser.
        /// </summary>
        BrowserId BrowserId { get; }
    }
}