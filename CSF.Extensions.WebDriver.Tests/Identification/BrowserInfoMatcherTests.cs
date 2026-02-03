using CSF.Extensions.WebDriver.Quirks;

namespace CSF.Extensions.WebDriver.Identification;

[TestFixture,Parallelizable]
public class BrowserInfoMatcherTests
{
    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueWhenOnlyTheBrowserNameMatches(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", MissingBrowserVersion.Instance);
        var browserInfo = new BrowserInfo { Name = "FooBrowser" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.True);
    }

    [Test,AutoMoqData]
    public void MatchesShouldUseACaseInsensitiveMatchForBrowserName(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FOOBROWSER", "BarPlatform", MissingBrowserVersion.Instance);
        var browserInfo = new BrowserInfo { Name = "foobrowser" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.True);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfTheBrowserNameDoesNotMatch(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", MissingBrowserVersion.Instance);
        var browserInfo = new BrowserInfo { Name = "BazBrowser" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.False);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfThePlatformIsSpecifiedAndDiffers(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", MissingBrowserVersion.Instance);
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BazPlatform" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.False);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfNameAndPlatformMatchAndNoVersionsAreSpecified(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", MissingBrowserVersion.Instance);
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BarPlatform" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.True);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfTheBrowserIsLowerThenTheMinVersion(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", SemanticBrowserVersion.Parse("1.2.3"));
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BarPlatform", MinVersion = "1.2.4" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.False);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfTheBrowserIsHigherThenTheMaxVersion(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", SemanticBrowserVersion.Parse("1.2.3"));
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BarPlatform", MaxVersion = "1.2.2" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.False);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfTheBrowserIsEqualToTheMinVersion(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", SemanticBrowserVersion.Parse("1.2.3"));
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BarPlatform", MinVersion = "1.2.3" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.True);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfTheBrowserIsEqualToTheMaxVersion(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", SemanticBrowserVersion.Parse("1.2.3"));
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BarPlatform", MaxVersion = "1.2.3" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.True);
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfTheBrowserIsBetweenTheMinAndMaxVersions(BrowserInfoMatcher sut)
    {
        var browserId = new BrowserId("FooBrowser", "BarPlatform", SemanticBrowserVersion.Parse("1.2.4"));
        var browserInfo = new BrowserInfo { Name = "FooBrowser", Platform = "BarPlatform", MinVersion = "1.2.3", MaxVersion = "1.2.5" };
        Assert.That(() => sut.Matches(browserId, browserInfo), Is.True);
    }
}