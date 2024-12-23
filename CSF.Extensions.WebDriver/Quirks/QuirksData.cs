using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF.Extensions.WebDriver.Quirks
{
    /// <summary>
    /// Gets information related to the WebDriver/browser quirks.
    /// </summary>
    public class QuirksData
    {
        IDictionary<string,BrowserInfoCollection> quirks = new Dictionary<string,BrowserInfoCollection>();

        /// <summary>
        /// Gets or sets a collection of the quirks, whereby each key is the name of a specific quirk.
        /// </summary>
        public IDictionary<string,BrowserInfoCollection> Quirks
        {
            get => quirks;
            set => quirks = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal QuirksData DeepCopy() => new QuirksData { Quirks = Quirks.ToDictionary(k => k.Key, v => v.Value.DeepCopy()) };

        /// <summary>
        /// Gets an empty instance of <see cref="QuirksData"/>.
        /// </summary>
        public static QuirksData Empty => new QuirksData();
    }
}