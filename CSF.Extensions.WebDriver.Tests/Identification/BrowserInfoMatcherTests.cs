namespace CSF.Extensions.WebDriver.Identification;

[TestFixture,Parallelizable]
public class BrowserInfoMatcherTests
{
    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueWhenOnlyTheBrowserNameMatches()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfTheBrowserNameDoesNotMatch()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfThePlatformIsSpecifiedAndDiffers()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfNameAndPlatformMatchAndNoVersionsAreSpecified()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfTheBrowserIsLowerThenTheMinVersion()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnFalseIfTheBrowserIsHigherThenTheMaxVersion()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfTheBrowserIsEqualToTheMinVersion()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfTheBrowserIsEqualToTheMaxVersion()
    {
        Assert.Fail("Write this test");
    }

    [Test,AutoMoqData]
    public void MatchesShouldReturnTrueIfTheBrowserIsBetweenTheMinAndMaxVersions()
    {
        Assert.Fail("Write this test");
    }
}