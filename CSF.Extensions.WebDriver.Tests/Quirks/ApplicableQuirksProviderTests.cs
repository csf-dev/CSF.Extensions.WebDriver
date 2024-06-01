using AutoFixture.NUnit3;
using CSF.Extensions.WebDriver.Identification;
using Moq;

namespace CSF.Extensions.WebDriver.Quirks;

[TestFixture,Parallelizable]
public class ApplicableQuirksProviderTests
{
    [Test,AutoMoqData]
    public void GetApplicableQuirksShouldReturnCollectionOfMatchingQuirks([Frozen] IGetsBrowserInfoMatch browserMatcher,
                                                                          [Frozen] IGetsQuirksData quirksDataProvider,
                                                                          ApplicableQuirksProvider sut,
                                                                          BrowserInfoCollection browsersA,
                                                                          BrowserInfoCollection browsersB,
                                                                          BrowserInfoCollection browsersC,
                                                                          BrowserInfoCollection browsersD,
                                                                          BrowserInfo browserInfoA1,
                                                                          BrowserInfo browserInfoA2,
                                                                          BrowserInfo browserInfoB1,
                                                                          BrowserInfo browserInfoB2,
                                                                          BrowserInfo browserInfoC1,
                                                                          BrowserInfo browserInfoC2,
                                                                          BrowserInfo browserInfoD1,
                                                                          BrowserInfo browserInfoD2,
                                                                          string quirkA,
                                                                          string quirkB,
                                                                          string quirkC,
                                                                          string quirkD,
                                                                          BrowserId browserId)
    {
        browsersA.AffectedBrowsers = new HashSet<BrowserInfo> { browserInfoA1, browserInfoA2 };
        browsersB.AffectedBrowsers = new HashSet<BrowserInfo> { browserInfoB1, browserInfoB2 };
        browsersC.AffectedBrowsers = new HashSet<BrowserInfo> { browserInfoC1, browserInfoC2 };
        browsersD.AffectedBrowsers = new HashSet<BrowserInfo> { browserInfoD1, browserInfoD2 };
        var quirksSource = new QuirksData
        {
            Quirks = new Dictionary<string, BrowserInfoCollection>
            {
                { quirkA, browsersA },
                { quirkB, browsersB },
                { quirkC, browsersC },
                { quirkD, browsersD },
            }
        };
        var expectedMatching = new[] { browserInfoA2, browserInfoC1 };
        Mock.Get(quirksDataProvider).Setup(x => x.GetQuirksData()).Returns(quirksSource);
        Mock.Get(browserMatcher).Setup(x => x.Matches(browserId, It.Is<BrowserInfo>(bi => expectedMatching.Contains(bi)))).Returns(true);

        Assert.That(() => sut.GetApplicableQuirks(browserId), Is.EquivalentTo(new[] { quirkA, quirkC }));
    }
}