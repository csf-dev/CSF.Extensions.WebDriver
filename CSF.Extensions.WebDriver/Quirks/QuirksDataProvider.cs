using System;
using Microsoft.Extensions.Options;

namespace CSF.Extensions.WebDriver.Quirks
{
    public class QuirksDataProvider : IGetsQuirksData
    {
        readonly QuirksData data;

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
        public QuirksDataProvider(QuirksData quirksData)
        {
            if (quirksData is null) throw new ArgumentNullException(nameof(quirksData));
            
            data = MergeQuirksData(quirksData);
        }
        
        public QuirksDataProvider(IOptions<QuirksData> optionsData, QuirksData quirksData)
        {
            if (optionsData is null) throw new ArgumentNullException(nameof(optionsData));
            if (quirksData is null) throw new ArgumentNullException(nameof(quirksData));

            data = MergeQuirksData(optionsData.Value, quirksData);
        }
    }
}