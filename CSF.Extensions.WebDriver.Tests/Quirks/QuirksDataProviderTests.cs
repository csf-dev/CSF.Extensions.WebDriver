using Microsoft.Extensions.Options;
using Moq;

namespace CSF.Extensions.WebDriver.Quirks;

[TestFixture,Parallelizable]
public class QuirksDataProviderTests
{
    [Test,AutoMoqData]
    public void GetQuirksDataShouldReturnCopyOfOptionsDataIfOnlyOptionsProvided(QuirksData data, IOptions<QuirksData> options)
    {
        data.Quirks.Add("sampleQuirk", new() { AffectedBrowsers = new HashSet<BrowserInfo> { new() { Name = "FooBrowser" } } });
        Mock.Get(options).SetupGet(x => x.Value).Returns(data);

        var sut = new QuirksDataProvider(options);
        var result = sut.GetQuirksData();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.SameAs(data), "Data must not be the same object; it must be a copy.");
            Assert.That(result.Quirks, Contains.Key("sampleQuirk"), "Data includes a quirk of the expected key");
            Assert.That(() => result.Quirks["sampleQuirk"].AffectedBrowsers,
                        Has.One.Matches<BrowserInfo>(bi => bi.Name == "FooBrowser"),
                        "Data includes an expected data value");
        });
    }

    [Test,AutoMoqData]
    public void GetQuirksDataShouldReturnCopyOfStaticDataIfOnlyDataProvided(QuirksData data)
    {
        data.Quirks.Add("sampleQuirk", new() { AffectedBrowsers = new HashSet<BrowserInfo> { new() { Name = "FooBrowser" } } });

        var sut = new QuirksDataProvider(data);
        var result = sut.GetQuirksData();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.SameAs(data), "Data must not be the same object; it must be a copy.");
            Assert.That(result.Quirks, Contains.Key("sampleQuirk"), "Data includes a quirk of the expected key");
            Assert.That(() => result.Quirks["sampleQuirk"].AffectedBrowsers,
                        Has.One.Matches<BrowserInfo>(bi => bi.Name == "FooBrowser"),
                        "Data includes an expected data value");
        });
    }

    [Test,AutoMoqData]
    public void GetQuirksDataShouldBeAbleToMergeTwoQuirksTogether(QuirksData data1,
                                                                  IOptions<QuirksData> options,
                                                                  QuirksData data2,
                                                                  BrowserInfoCollection browsers1,
                                                                  BrowserInfoCollection browsers2)
    {
        data1.Quirks.Add("sampleQuirk", browsers1);
        data2.Quirks.Add("otherQuirk", browsers2);
        Mock.Get(options).SetupGet(x => x.Value).Returns(data1);

        var sut = new QuirksDataProvider(options, data2);
        var result = sut.GetQuirksData();

        Assert.That(result.Quirks, Contains.Key("sampleQuirk").And.ContainKey("otherQuirk"), "Data includes both expected quirk keys");
    }

    [Test,AutoMoqData]
    public void GetQuirksDataShouldAllowOptionsDataToShadowStaticData(QuirksData data1,
                                                                  IOptions<QuirksData> options,
                                                                  QuirksData data2,
                                                                  BrowserInfoCollection browsers1,
                                                                  BrowserInfoCollection browsers2)
    {
        browsers1.AffectedBrowsers = new HashSet<BrowserInfo> { new() { Name = "FooBrowser" } };
        browsers2.AffectedBrowsers = new HashSet<BrowserInfo> { new() { Name = "BarBrowser" } };
        data1.Quirks.Add("sampleQuirk", browsers1);
        data2.Quirks.Add("sampleQuirk", browsers2);
        Mock.Get(options).SetupGet(x => x.Value).Returns(data1);

        var sut = new QuirksDataProvider(options, data2);
        var result = sut.GetQuirksData();

        Assert.That(result.Quirks["sampleQuirk"].AffectedBrowsers, Has.Count.EqualTo(1).And.One.Matches<BrowserInfo>(bi => bi.Name == "FooBrowser"));
    }
}